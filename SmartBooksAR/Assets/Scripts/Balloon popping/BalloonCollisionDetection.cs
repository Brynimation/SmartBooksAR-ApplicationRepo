using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;



//This class is used to determine if there is a screen-space collision between the answers and the target positioned at the centre of the user's face.
//This class is attached to the user's face renderer.
public class BalloonCollisionDetection : CollisionDetection
{
    public GameObject correctAnswerSelectedExplosion;
    public Color incorrectAnswerFadeColour;
    public float balloonFadeTime;
    protected override void Select()
    {
        Ray ray = CastRayThroughBoundsCentre();
        RaycastHit hit;
        //A spherecast acts like a "thick" raycast - we project a sphere with a radius equal to the radius of the target on the user's face along the ray.
        //If any answers collide with this projected sphere, then the answer is destroyed.
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude/2f, out hit, answerLayerMask)) 
        {
            GameObject hitGo = hit.collider.gameObject;
            string name = hitGo.name;
            string tag = hitGo.tag;
            int scoreChange = tag == "Correct" ? 1 : -1;
            if (tag == "Correct")
            {
                Instantiate(correctAnswerSelectedExplosion, hitGo.transform.position, Quaternion.Euler(90f, 0f, 0f));
                Balloon.OnDestroyBalloon?.Invoke(scoreChange);
                Destroy(hitGo);
            }
            else {
                Balloon incorrectBalloon = hitGo.GetComponent<Balloon>();
                Balloon.OnDestroyBalloon?.Invoke(scoreChange);
                incorrectBalloon.IncorrectAnswerFade(incorrectAnswerFadeColour, balloonFadeTime);
            }


        }

    }
    private void Update()
    {
        Select();
    }
}
