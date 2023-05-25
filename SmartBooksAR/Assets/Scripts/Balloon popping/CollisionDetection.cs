using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;



//This class is used to determine if there is a screen-space collision between the answers and the target positioned at the centre of the user's face.
//This class is attached to the user's face renderer.
public class CollisionDetection : MonoBehaviour
{
    [SerializeField] LayerMask answerLayerMask;
    public MeshRenderer mRenderer; 
    public GameObject correctAnswerSelectedExplosion;
    public GameObject incorrectAnswerSelectedExplosion;
    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        Vector3 centreScreenSpace = Camera.main.WorldToScreenPoint(mRenderer.bounds.center);
        Ray ray = Camera.main.ScreenPointToRay(centreScreenSpace);
        RaycastHit hit;

        
        //A spherecast acts like a "thick" raycast - we project a sphere with a radius equal to the radius of the target on the user's face along the ray.
        //If any answers collide with this projected sphere, then the answer is destroyed.
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude/2f, out hit, answerLayerMask)) 
        {
            string name = hit.collider.gameObject.name;
            string tag = hit.collider.tag;
            int scoreChange = tag == "Correct" ? 1 : -1;
            GameObject explosion = tag == "Correct" ? correctAnswerSelectedExplosion : incorrectAnswerSelectedExplosion;
            Instantiate(explosion, hit.collider.transform.position, Quaternion.Euler(90f, 0f, 0f));
            Balloon.OnDestroyBalloon?.Invoke(scoreChange);
            Destroy(hit.collider.gameObject);
        }

    }
}
