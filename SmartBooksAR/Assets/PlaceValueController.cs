using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System.Linq;
using System;
using System.Runtime.CompilerServices;

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
    public float minDistThreshold;
    [SerializeField] float changeColourTime;
    [SerializeField] Color selectedColour;
    [SerializeField] List<GameObject> prefabsToInstantiate;
    [SerializeField] List<PlaceValueQuestion> placeValueQuestions;

    Dictionary<string, Detector> arObjects = new Dictionary<string, Detector>();
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
        UpdateARImages();
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
                Debug.Log(image.referenceImage.name);
            }
        }
        bool correctOrder = checkNumberIsCorrect(orderedTrackables, placeValueQuestions[currentQuestionIndex].targetNumber, alreadyChecked);
        if (correctOrder)
        {
            for (int i = 1; i < orderedTrackables.Count; i++) 
            {
                if (!ImagesAreCloseEnough(orderedTrackables[i - 1], orderedTrackables[i])) 
                {
                    return;
                }
            }
            alreadyChecked = true;
            StartCoroutine(SpawnNextQuestion());
        }
        else {
            //Debug.Log("false");
        }
    }

    IEnumerator SpawnNextQuestion() 
    {
        yield return new WaitForSeconds(1f);
        foreach (Detector detector in arObjects.Values) 
        {
            detector.ChangeColour(selectedColour, changeColourTime);
        }
        yield return new WaitForSeconds(changeColourTime);
        alreadyChecked = false;
        currentQuestionIndex++;
        if (currentQuestionIndex >= placeValueQuestions.Count)
        {
            OnSpawnLargeText?.Invoke("Finished!");
            OnSpawnNextQuestion?.Invoke("");
        }
        else 
        {
            OnSpawnNextQuestion?.Invoke(placeValueQuestions[currentQuestionIndex].question);
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
        
    }
}
