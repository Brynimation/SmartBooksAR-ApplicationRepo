using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactor : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    Interactable curSelectedInteractable;
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("touched!");
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, layerMask))
            {
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
                curSelectedInteractable.Rotate(touch.deltaPosition);
            }
        }
    }
}
