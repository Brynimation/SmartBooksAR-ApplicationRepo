using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    PlaceValueController placeValueController;
    Material material;
    Color originalColour;
    void Start()
    {
        placeValueController = FindObjectOfType<PlaceValueController>();
        material = GetComponent<Renderer>().material;
        originalColour = material.color;
        placeValueController.OnSpawnNextQuestion += ReturnToOriginalColour;
    }

    public void ChangeColour(Color newColour, float colourChangeTime)
    {
        StartCoroutine(ChangeColourCoroutine(newColour, colourChangeTime));
    }
    IEnumerator ChangeColourCoroutine(Color newColour, float colourChangeTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < colourChangeTime) 
        {
            material.color = Color.Lerp(originalColour, newColour, elapsedTime/colourChangeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void ReturnToOriginalColour(string message) 
    {
        material.color = originalColour;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
