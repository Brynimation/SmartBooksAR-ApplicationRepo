using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrueFalseRegion : MonoBehaviour
{
    public AnimationCurve colourChangeCurve;
    public float ChangeColourTime;
    public Color CorrectColour;
    public Color IncorrectColour;
    public Color trueColour;
    public Color falseColour;
    public Vector2 viewportPosition;
    public float distFromCamera;
    public TMP_Text tfText;
    public bool IsTrue {
        get 
        {
            return isTrue;
        }
        set 
        {
            isTrue = value;
            originalColour = value ? trueColour : falseColour;
            //StartCoroutine(SetSelectedCoroutine(originalColour, 1f));
        }
    }
    private Material mat;
    private Color selectedColour;
    private Color originalColour;
    private bool isTrue;

    public void SetSelected(bool isCorrect) 
    {
        selectedColour = isCorrect ? CorrectColour : IncorrectColour;
        StartCoroutine(SetSelectedCoroutine(originalColour, ChangeColourTime));
    }
    private IEnumerator SetSelectedCoroutine(Color colour, float ChangeColourTime) 
    {
        float elapsedTime = 0f;
        while (elapsedTime < ChangeColourTime) 
        {
            mat.color = Color.Lerp(colour, selectedColour, colourChangeCurve.Evaluate(elapsedTime / ChangeColourTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
            
    }
    void Awake()
    {
        mat = GetComponent<Renderer>().material;
#if !UNITY_EDITOR
        Vector3 localScale = tfText.transform.localScale;
        localScale.x *= -1;
        tfText.transform.localScale = localScale;
#endif


    }
    private void Start()
    {
        mat.color = originalColour = IsTrue ? trueColour : falseColour;
        string text = isTrue ? "True" : "False";
        tfText.SetText(text);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPos = new Vector3(viewportPosition.x, viewportPosition.y, distFromCamera);
        transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
    }
}
