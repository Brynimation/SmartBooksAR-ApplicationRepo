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
    public Vector2 leftRightViewportPosition;
    public float distFromCamera;
    public TMP_Text tfTopText;
    public TMP_Text tfBottomText;
    public Vector2 upDownYViewportPos;

    Vector2 viewportPosition;

    TrueFalseCollisionDetection tfCollisionDetection;
    bool detected = false;
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
        if (isTrue)
        {
            viewportPosition = new Vector2(leftRightViewportPosition.x, upDownYViewportPos.x);
        }
        else {
            viewportPosition = new Vector2(leftRightViewportPosition.y, upDownYViewportPos.y);
        }

#if !UNITY_EDITOR
        Vector3 localScale = tfText.transform.localScale;
        localScale.x *= -1;
        tfTopText.transform.localScale = localScale;
        tfBottomText.transform.localScale = localScale;
#endif
        

    }
    private void Start()
    {
        mat.color = originalColour = IsTrue ? trueColour : falseColour;
        string text = isTrue ? "True" : "False";
        tfTopText.SetText(text);
        tfBottomText.SetText(text);
    }

    private void TryGetTrueFalseCollisionDetection() 
    {
        if (tfCollisionDetection == null) detected = false;
        if (detected) return;
        tfCollisionDetection = FindObjectOfType<TrueFalseCollisionDetection>();
        if (tfCollisionDetection != null)
        {
            tfCollisionDetection.OnSelectAnswer += StartChangePosition;
            detected = true;
        }
    }
    void StartChangePosition(bool isTrue) 
    {
        StartCoroutine(ChangePosition());
    }
    IEnumerator ChangePosition() 
    {
        yield return new WaitForSeconds(0.5f);
        viewportPosition.y = (viewportPosition.y == upDownYViewportPos.x) ? upDownYViewportPos.y : upDownYViewportPos.x;
    }
    
    // Update is called once per frame
    void Update()
    {
        TryGetTrueFalseCollisionDetection();
        Vector3 viewportPos = new Vector3(viewportPosition.x, viewportPosition.y, distFromCamera);
        transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
    }
}
