using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;

public class Tile : MonoBehaviour
{
    public Vector2 ViewportPosition;
    public float distanceFromNearClipPlane;
    public Canvas worldCanvas;
    public Image image;
    public float glowIntensity = 4f;
    public float glowFadeTime= 5f;
    public bool correct;
    public static Action<bool> OnSelected;
    public Color originalColour;
    public Color selectedColour;
    public Shader dissolveShader;
    public CanvasGroup imageCanvasGroup;

    Material material;
    Renderer rend;
    TileCollisionDetection tileDetection;
    float selectionTime;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        image = GetComponentInChildren<Image>();
        material = rend.material;

        //As the face camera is being used, in builds we must flip the direction of the text.
    #if !UNITY_EDITOR
            Vector3 localScale = image.transform.localScale;
            localScale.x *= -1;
            image.transform.localScale = localScale;
    #endif
        originalColour = rend.material.color;
    }
    void Update()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(ViewportPosition.x, ViewportPosition.y, Camera.main.nearClipPlane + distanceFromNearClipPlane));
    }

    //Initialises the position and image on a tile.
    public void InitialiseAnswer(Vector2 ViewportPosition, Sprite answerSprite, int curIndex, List<int> correctIndices)
    {
        this.ViewportPosition = ViewportPosition;
        this.image.sprite = answerSprite;
        correct = correctIndices.Contains(curIndex);
        tag = correct ? "Correct" : "Incorrect";
    }

    public void SetSelected() 
    {
        gameObject.layer = LayerMask.NameToLayer("Selected");
        if (correct)
        {
            //Makes the tile glow green
            StartCoroutine(StartGlow(glowFadeTime, selectedColour));
        }
        else {
            //Makes the tile dissolve with an orange glow and shrink
            StartCoroutine(Dissolve(glowFadeTime));
        }

    }

    //Coroutine that is called if the selected answer is incorrect; causes the tile to dissolve and shrink.
    IEnumerator Dissolve(float dissolveTime) 
    {
        Material mat = new Material(dissolveShader);
        mat.SetColor("_BaseColour", originalColour);

        mat.SetFloat("_AlphaClip", 1);//Here we enable alpha clipping (pixels below a certain alpha threshold are culled).
                                      // see the shader "ShaderGraph/Dissolve" and this tutorial (https://www.youtube.com/watch?v=taMp1g1pBeE&t=513s) for how the dissolve effect works.
        mat.EnableKeyword("_ALPHATEST_ON");
        rend.material = mat;
        mat.SetFloat("_Dissolve", 0f);

        float elapsedTime = 0f;
        Vector3 localScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        while (elapsedTime < dissolveTime) 
        {
            float lerpPercent = (float)Math.Round(elapsedTime / dissolveTime, 2);
            rend.material.SetFloat("_Dissolve", (float) lerpPercent); //The dissolve property of the shader gradually makes the tile disappear.
            transform.localScale = Vector3.Lerp(localScale, targetScale, lerpPercent); //We also shrink the tile as it dissolves
            imageCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime/dissolveTime);  //We also decrease the opacity of the text on the tile to zero
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        OnSelected?.Invoke(correct); 
        Destroy(this.gameObject);
    }
    //Coroutine that is called if the selected answer is correct; causes the tile to glow green
    IEnumerator StartGlow(float glowFadeTime, Color colour) 
    {
        material.EnableKeyword("_EmissionColor"); //Here we enable emission so that our tiles glow
        material.EnableKeyword("_EMISSION");
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        float curIntensity = 0f;
        float elapsedTime = 0f;
        while (curIntensity < glowIntensity) 
        {
            curIntensity = Mathf.Lerp(0f, glowIntensity, (float)elapsedTime /(float) glowFadeTime);
            material.SetColor("_EmissionColor", colour * curIntensity);
            elapsedTime += Time.deltaTime;
            yield return null;
              
        }
        material.SetColor("_EmissionColor", colour * glowIntensity);
        OnSelected?.Invoke(correct);
    }
    public void SetColour(Color colour) 
    {
        material.SetColor("_BaseColor", colour);  
    }

}
