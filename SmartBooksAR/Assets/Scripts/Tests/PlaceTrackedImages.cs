using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour
{
    private ARTrackedImageManager trackedImagesManager; //Reference to the ARTrackedImageManager component attached to our gameobject

    //List of prefabs to instantiate. They should be named the same as their corresponding 2D images in the reference image library
    //This could be as simple as a quad with an image texture applied to it, or something as complex as an animated 3d model
    //Each one of these prefabs maps to an image in our imageReferenceLibrary
    public GameObject[] arPrefabs;
    //Dictionary of created prefabs
    private readonly Dictionary<string, GameObject> instantiatedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() 
    {
        //the trackedImagesChanged event is invoked once per frame with information about the ARTrackedImages that have changed (added, updated or removed). We subscribe to this event with our
        //OnTrackedImagesChanged method, which will hence be called whenever this method is invoked
        trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) 
    {
        //loop through all tracked images that have been detected
        foreach (ARTrackedImage trackedImage in eventArgs.added) 
        {
            string imageName = trackedImage.referenceImage.name;

            //compare the name of each prefab in our list of arPrefabs to the current name.
            foreach (GameObject curPrefab in arPrefabs) 
            {
                //if the image name DOES match the current prefab and HASN'T already been instantiated
                if (string.Compare(curPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0 && !instantiatedPrefabs.ContainsKey(imageName)) 
                {
                    //instantiate the prefab, parenting it to the ARTrackedImage
                    GameObject newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    //Add the created prefab to our dictionary
                    instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }
        //For all prefabs that have been created so far, set them active or not depending on whether their corresponding image is currently being tracked
        //Some objects that briefly leave the field of view will be set to inactive with this code.
        foreach (ARTrackedImage image in eventArgs.updated) {
            instantiatedPrefabs[image.referenceImage.name].SetActive(image.trackingState == TrackingState.Tracking);
        }
        //If the AR subsystem has given up looking for a tracked image
        foreach (ARTrackedImage trackedImage in eventArgs.removed) 
        {
            //Destroy this tracked image's prefab
            Destroy(instantiatedPrefabs[trackedImage.referenceImage.name]);
            //remove the instance from our dictionary
            instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
            //(alternatively, you can set the prefab instance to inactive).
        }
    } 
}

