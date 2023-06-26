using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

[System.Serializable]
public struct PlaceValueQuestion
{
    public string question;
    public string targetNumber;
    public Texture2D numberImage;
};

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceValueController : MonoBehaviour
{

    public Action<string> OnSpawnNextQuestion;
    public Action<string> OnSpawnLargeText;
    public Action<int> OnSelectAnswer;
    public float minDistThreshold;
    [SerializeField] float changeColourTime;
    [SerializeField] Color selectedColour;
    [SerializeField] Color incorrectColour;
    [SerializeField] float incorrectFlashTime;
    [SerializeField] List<GameObject> prefabsToInstantiate;
    [SerializeField] List<PlaceValueQuestion> placeValueQuestions;
    [SerializeField] CelebrationDragonController celebrationDragon;
    [SerializeField] GameObject dragonInstantiationExplosion;
    

    Dictionary<string, Detector> arObjects = new Dictionary<string, Detector>();
    List<ARTrackedImage> prevOrderedTrackables;
    List<ARTrackedImage> orderedTrackables;

    private ARTrackedImageManager imageManager;
    private List<GameObject> instantiated= new List<GameObject>();
    private int currentQuestionIndex;
    private bool started = false;
    private bool alreadyChecked;
    private void Awake()
    {
        imageManager= GetComponent<ARTrackedImageManager>();
        foreach (GameObject currentPrefab in prefabsToInstantiate) 
        {
            Detector newARObject = Instantiate(currentPrefab, Vector3.zero, Quaternion.identity).GetComponent<Detector>();
            newARObject.name = currentPrefab.name;
            arObjects.Add(currentPrefab.name, newARObject);
        }
        currentQuestionIndex = 0;
      
        orderedTrackables = new List<ARTrackedImage>();
        prevOrderedTrackables = new List<ARTrackedImage>();
        prevOrderedTrackables.InsertRange(0, orderedTrackables);
    }
    IEnumerator DelayStart() 
    {
        OnSpawnLargeText?.Invoke("3");
        yield return new WaitForSeconds(1f);
        OnSpawnLargeText?.Invoke("2");
        yield return new WaitForSeconds(1f);
        OnSpawnLargeText?.Invoke("1");
        yield return new WaitForSeconds(1f);
        OnSpawnLargeText("Start!");
        yield return new WaitForSeconds(1f);
        started = true;
        OnSpawnNextQuestion?.Invoke(placeValueQuestions[currentQuestionIndex].question);
    }
    void Start() 
    {
        StartCoroutine(DelayStart());
    }

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private bool ImagesAreCloseEnough(ARTrackedImage image1, ARTrackedImage image2)
    {
        return (Mathf.Abs(image1.transform.position.y - image2.transform.position.y) < minDistThreshold);
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) 
    {
        if (!started) return;
        foreach (ARTrackedImage trackedImage in eventArgs.added) 
        {
            Debug.Log(trackedImage.referenceImage.name + " is added");
            AddARImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in imageManager.trackables) 
        {
            if (!orderedTrackables.Contains(trackedImage)) 
            {
                AddARImage(trackedImage);
            }
        }
        /*foreach (ARTrackedImage trackedImage in eventArgs.updated) 
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
            {
                Debug.Log(trackedImage.referenceImage.name + " has state: " + trackedImage.trackingState);
            }
           
        }*/
        //UpdateARImages();
        foreach (ARTrackedImage trackedImage in eventArgs.removed) 
        {
            Debug.Log(trackedImage.referenceImage.name + " is removed");
            arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            orderedTrackables.Remove(trackedImage);
        }
    }

    void AddARImage(ARTrackedImage trackedImage) 
    {
        if (prefabsToInstantiate != null) 
        {
            arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
            //arObjects[trackedImage.referenceImage.name].transform.position = Vector3.zero;
            orderedTrackables.Add(trackedImage);
        }

    }

    void UpdateARImages() 
    {
        foreach (var trackedImage in imageManager.trackables) 
        {
            //Debug.Log(trackedImage.referenceImage.name + " is updated");
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
                arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
                arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
            }
            else {
                arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
                orderedTrackables.Remove(trackedImage);
            }

        }
        orderedTrackables = orderedTrackables.OrderBy(trackable => trackable.transform.position.x).ToList<ARTrackedImage>();
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
            for (int i = 0; i < orderedTrackables.Count; i++) 
            {
                ARTrackedImage image = orderedTrackables[i];
            }
        }

        //Check that all the cards are close enough to each other for an answer to be registered
        if (orderedTrackables.Count < 3) return;
        //Debug.Log("current question index vs questions count: "+currentQuestionIndex + " " + placeValueQuestions.Count);
        for (int i = 1; i < orderedTrackables.Count; i++) 
        {
            //Debug.Log("ordered tracakbles index: " + i + " is: " + orderedTrackables[i]);
            if (!ImagesAreCloseEnough(orderedTrackables[i - 1], orderedTrackables[i]))
            {
                return;
            }
        }
        //If they're close enough, check they're in the correct order
        bool correctOrder = checkNumberIsCorrect(orderedTrackables, placeValueQuestions[currentQuestionIndex].targetNumber, alreadyChecked);
        if (correctOrder && !alreadyChecked)
        {
            Debug.Log("Here I am!");
            alreadyChecked = true;
            StartCoroutine(SpawnNextQuestion());

        }
        else if(!alreadyChecked){
            alreadyChecked = true;
            StartCoroutine(IncorrectAnswerTransition());
        }
        if (OrderChanged(orderedTrackables, prevOrderedTrackables)) 
        {
            alreadyChecked = false;
        }
        prevOrderedTrackables.Clear();
        prevOrderedTrackables.InsertRange(0, orderedTrackables);

    }
    bool OrderChanged(List<ARTrackedImage> cur, List<ARTrackedImage> prev) 
    {
        if (cur == null || prev == null || cur.Count != prev.Count) return false;
        for (int i = 0; i < cur.Count; i++) 
        {
            if (cur[i] != prev[i]) return true;
        }
        return false;
    }

    IEnumerator IncorrectAnswerTransition() 
    {
        OnSelectAnswer?.Invoke(-1);
        foreach (Detector detector in arObjects.Values)
        {
            detector.ChangeColour(incorrectColour, incorrectFlashTime);
        }
        yield return new WaitForSeconds(incorrectFlashTime);
        foreach (Detector detector in arObjects.Values)
        {
            detector.ReturnToOriginalColour("");
        }

    }
    public void InvokeSpawnNextQuestionEvent() 
    {
        OnSpawnNextQuestion?.Invoke(placeValueQuestions[currentQuestionIndex].question);
    }
    IEnumerator SpawnNextQuestion() 
    {
        OnSelectAnswer?.Invoke(1);
        yield return new WaitForSeconds(1f);
        foreach (Detector detector in arObjects.Values) 
        {
            detector.ChangeColour(selectedColour, changeColourTime);
        }
        yield return new WaitForSeconds(changeColourTime);
        Debug.Log("this is also current question index: "+currentQuestionIndex);
        currentQuestionIndex++;
        if (currentQuestionIndex >= placeValueQuestions.Count)
        {
            OnSpawnLargeText?.Invoke("Finished!");
            OnSpawnNextQuestion?.Invoke("");
        }
        else 
        {
            //Debug.Log("ELSE!");
            CelebrationDragonController dragon = Instantiate(celebrationDragon, orderedTrackables[1].gameObject.transform.position, Quaternion.identity).GetComponent<CelebrationDragonController>();
            GameObject confettiExplosion = Instantiate(dragonInstantiationExplosion, dragon.transform.position, Quaternion.identity);
            confettiExplosion.transform.localScale = Vector3.one * 0.2f;
            yield return new WaitForSeconds(2f);
            dragon.Jump();
            yield return new WaitForSeconds(2f);
            dragon.SetSelected();
            yield return new WaitForSeconds(1f);
            InvokeSpawnNextQuestionEvent();
        }
        
    }

    private bool checkNumberIsCorrect(List<ARTrackedImage> trackables, string targetNumber, bool alreadyChecked) 
    {
        if (targetNumber.Length != trackables.Count || alreadyChecked) return false;
        for (int i = 0; i < targetNumber.Length; i++) 
        {
            string targetString = targetNumber[i] + "ReferenceImage";
            if (targetString != trackables[i].referenceImage.name) 
            {
                return false;
            }
        }
        Debug.Log("true!");
        return true;
    } 
    // Update is called once per frame
    void Update()
    {
        UpdateARImages();
    }
}
