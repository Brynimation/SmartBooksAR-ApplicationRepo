using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class ARCursor : MonoBehaviour
{
    public GameObject cursorChildObject;
    public GameObject objectToPlace;
    public ARRaycastManager raycastManager; //detects planes we touch

    public bool useCursor;
    void Start()
    {
        cursorChildObject.SetActive(useCursor);
    }

    // Update is called once per frame
    void Update()
    {
        if (useCursor) 
        {
            UpdateCursor();
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
            if (useCursor) 
            {
                //if we're using the cursor, instantiate the object at the cursor's position
                GameObject.Instantiate(objectToPlace, transform.position, transform.rotation);
            }
            else 
            {
                //if we're not using the cursor, instantiate the object where we touch
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

                if (hits.Count > 0) 
                {
                    GameObject.Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                }
            }
        }
    }
    void UpdateCursor() 
    {
        Vector2 screenPos = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f)); //(0.5, 0.5) is the centre of the viewport
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        //A trackable is a feature in the physical environment that a device is able to track. Here, we only want to track planes
        raycastManager.Raycast(screenPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if(hits.Count > 0 ) 
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }
}
