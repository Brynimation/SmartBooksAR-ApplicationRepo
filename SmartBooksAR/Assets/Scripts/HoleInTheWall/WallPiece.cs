using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class ObjectViewportSpace : MonoBehaviour 
{

}
public class WallPiece : MonoBehaviour
{
    public static Action<int> OnPieceSelected;

    public int incrementDecrementValue;
    public float viewportPosY;
    public float distanceFromNearClipPlane;
    public float screenTime = 5f;

    [SerializeField] Canvas worldCanvas;
    [SerializeField] TMP_Text incrementDecrementText;
    

    Material mat;
    Camera cam;

    float elapsedTime;
    bool increment;
    void Awake()
    {
        cam = Camera.main;
        mat = GetComponent<Renderer>().material;
        Debug.Log("material: " + mat);

#if !UNITY_EDITOR
        Vector3 localScale = incrementDecrementText.transform.localScale;
        localScale.x *= -1;
        incrementDecrementText.transform.localScale = localScale;
#endif
    }
    public void InitialiseAnswer(float viewportPosY, int incrementDecrementValue, float distanceFromNearClipPlane, Color colour)
    {
        this.viewportPosY = viewportPosY;
        this.incrementDecrementValue = incrementDecrementValue;
        mat.color = colour;
        this.distanceFromNearClipPlane = distanceFromNearClipPlane;
        string symbol = (incrementDecrementValue > 0) ? "+" : "";
        incrementDecrementText.SetText(symbol + incrementDecrementValue.ToString());
    }

    public void SetSelected(Color selectedColour) 
    {
        mat.color = selectedColour;
        OnPieceSelected?.Invoke(incrementDecrementValue);
    }
}
