using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FaceSwap : MonoBehaviour
{
    public List<Material> faceMaterials;

    private ARFaceManager faceManager;
    private int faceMaterialIndex = 0;
    void Start()
    {
        faceManager = GetComponent<ARFaceManager>();
    }

    // Update is called once per frame
    public void SwitchFace() 
    {
        faceMaterialIndex+=1;
        faceMaterialIndex %= faceMaterials.Count;

        foreach (ARFace face in faceManager.trackables) 
        {
            face.gameObject.GetComponent<Renderer>().material = faceMaterials[faceMaterialIndex];
        }
    }
}
