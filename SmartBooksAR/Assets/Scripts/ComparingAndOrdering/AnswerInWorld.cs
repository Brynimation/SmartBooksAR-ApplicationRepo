using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class AnswerInWorld : MonoBehaviour
{
    //Events 
    public static Action<bool, int> OnSelectAnswer;//invoked whenever this answer is selected
    
    public float colourChangeTime = 1f;
    //public float floatAmplitude = 1f;
    //public float floatFrequency = 0.5f;
    public static int numAnswers;

    private int answerId;
    int answerValue;
    Transform cameraT;
    [SerializeField] TMP_Text answerText;
    public bool Correct
    {
         get { return correct; }
         set { correct = value; }
    }

    private bool correct;
    private void OnEnable()
    {
        numAnswers += 1;
        answerId = numAnswers;
    }
    private void OnDisable()
    {
        numAnswers -= 1;
    }
    void Start()
    {
        cameraT = Camera.main.transform;
    }
    public void InitialiseAnswer(string answerString, bool correct) 
    {
        this.correct = correct;
        //floatAmplitude = UnityEngine.Random.Range(0.1f, 0.25f);
        //floatFrequency = 1f;
         int.TryParse(answerString, out answerValue);
        answerText.SetText(answerString);
    }

    //Called by WorldAnswerSelection when this number is selected
    public void SetSelected() 
    {
        Color newColour = correct ? Color.green : Color.red;
        gameObject.layer = LayerMask.NameToLayer("Selected"); //We change the layer this number is on so it can't be selected by WorldAnswerSelection again
        StartCoroutine(ChangeColour(answerText.color, newColour, colourChangeTime));
    }

    //coroutine that changes the colour of this answer
    IEnumerator ChangeColour(Color originalColour, Color newColour, float changeTime) 
    {
        float elapsedTime = 0f;
        while (elapsedTime < changeTime) 
        {
            elapsedTime += Time.deltaTime;
            answerText.color = Color.Lerp(originalColour, newColour, elapsedTime / changeTime);
            yield return null;
        }
        OnSelectAnswer?.Invoke(Correct, answerValue);

    }
    // Update is called once per frame
    void Update() 
    {
        //make the number float up and down
        /*Vector3 upDir = answerText.transform.up.normalized;
        Vector3 position = transform.position;
        position += upDir * floatAmplitude * Time.deltaTime * Mathf.Sin(answerId + floatFrequency * Time.time);
        transform.position = position;*/
    }
    //In the LateUpdate function we turn the numbers into "billboard sprites"; a 2D image that always faces the camera.
    void LateUpdate()
    {
        transform.rotation = cameraT.rotation; 
        //transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
