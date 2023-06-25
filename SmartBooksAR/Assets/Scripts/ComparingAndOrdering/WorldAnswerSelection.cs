using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This class registers screen touches and checks whether or not an answer has been selected.
public class WorldAnswerSelection : MonoBehaviour
{

    public Action<bool> OnSelectAnswer;
    public LayerMask answerLayerMask;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount > 0) //if the user is touching their device's screen 
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began) //if the user just started touching the screen 
            {
                Ray ray = cam.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, answerLayerMask))
                {
                    AnswerInWorld answer = hit.collider.GetComponent<AnswerInWorld>();
                    DragonController dragon = answer.transform.parent.GetComponent<DragonController>();
                    if (answer != null) 
                    {
                        answer.SetSelected();
                        OnSelectAnswer?.Invoke(answer.Correct);
                        dragon.SetSelected(answer.Correct);
                    }
                }
            }
        }
    }
}
