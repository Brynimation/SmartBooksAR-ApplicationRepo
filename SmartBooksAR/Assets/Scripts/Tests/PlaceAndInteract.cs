using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceAndInteract : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject objectToPlace;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARPlaneManager planeManager;
    Interactable curSelectedInteractable;
    void Start()
    {
    }


    void Update()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("touched!");
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, layerMask))
            {
                Debug.Log("raycast");
                Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    curSelectedInteractable = interactable;
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            curSelectedInteractable.SetActive(true);
                            break;
                        case TouchPhase.Ended:
                            curSelectedInteractable.SetActive(false);
                            curSelectedInteractable = null;
                            break;
                        default:
                            break;

                    }
                }
            }
            if (curSelectedInteractable != null)
            {
                Debug.Log("move");
                curSelectedInteractable.Rotate(touch.deltaPosition);
            }
            else if (curSelectedInteractable == null && touch.phase == TouchPhase.Began) {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes)) 
                {
                    Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                }
                
            
            }
        }
    }
}