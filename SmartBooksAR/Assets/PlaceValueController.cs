using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceValueController : MonoBehaviour
{
    [SerializeField] GameObject prefabToInstantiate;
    
    private ARTrackedImageManager imageManager;
    private List<GameObject> instantiated= new List<GameObject>();
    private void Awake()
    {
        imageManager= GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) 
    {
        foreach (ARTrackedImage image in eventArgs.added) 
        {
            Debug.Log("found: "+ image.name);
            GameObject instantiated = Instantiate(prefabToInstantiate, image.transform);
            instantiated.transform.SetParent(image.transform);
        }
        foreach (var trackable in imageManager.trackables) 
        {
            Debug.Log(trackable.transform.name + " tracking state: " + trackable.trackableId);
        }
        foreach (ARTrackedImage image in eventArgs.updated) 
        {

        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
