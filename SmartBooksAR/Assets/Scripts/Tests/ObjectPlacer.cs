using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] GameObject objectToPlace;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARPlaneManager planeManager;
    void Start()
    {
    }


    void Update()
    {
        //If the number of fingers on the screen is greater than zero and we've just started touching the screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            //Fire a ray through the camera to a point on the screen; the point the user touched
            raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

            if (hits.Count > 0) 
            {
                Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
            }
        }
    }
}
