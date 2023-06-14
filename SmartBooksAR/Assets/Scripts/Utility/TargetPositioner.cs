using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

//This class positions the target to the centre of the user's face.
public class TargetPositioner : MonoBehaviour
{
    public GameObject targetPrefab;

    ARFace ARFace;
    ARFaceManager manager;
    Renderer faceRenderer;
    GameObject target;


    IEnumerator TryGetFace() 
    {
        ARFace = FindObjectOfType<ARFace>();
        while (ARFace == null) 
        {
            ARFace = FindObjectOfType<ARFace>();
            yield return null;
        }
        faceRenderer = ARFace.gameObject.GetComponent<MeshRenderer>();
        if (target == null) 
        {
            target = Instantiate(targetPrefab, transform.position, Quaternion.identity);
        }
    }
    void Start()
    {
        manager = FindObjectOfType<ARFaceManager>();
        StartCoroutine(TryGetFace());
    }

    // Update is called once per frame
    void Update()
    {
        if (ARFace != null && faceRenderer != null && target != null)
        {
            target.transform.position = faceRenderer.bounds.center;
            target.transform.localScale = faceRenderer.bounds.extents / 2f;
        }
        else
        {
            if (ARFace == null) ARFace = FindObjectOfType<ARFace>();
            if (ARFace != null)
            {
                faceRenderer = ARFace.gameObject.GetComponent<Renderer>();
            }
            if (target != null)
            {
                target.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(2f, 0.5f));
                target.transform.localScale = Vector3.one / 10f;
            }

        }
    }
}
