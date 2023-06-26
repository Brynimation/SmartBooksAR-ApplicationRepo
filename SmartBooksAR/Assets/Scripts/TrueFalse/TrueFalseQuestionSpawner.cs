using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[System.Serializable]
public struct TrueFalseQuestionAnswer 
{
    public string question;
    public bool correctAnswer;
}
public abstract class QuestionSpawner : MonoBehaviour
{
    public Action<string> OnSpawnNextQuestion;
    public Action<string> OnDisplayLargeText;
    public Action<string> OnUpdateTimeRemaining;
    public Action OnFinished;
    public float GameTime;

    protected float remainingTime;
    protected bool started;
    protected bool coroutineComplete;
    protected ARFace ARFace;
    protected Renderer faceRenderer;

    protected virtual void Start()
    {
        StartCoroutine(DelayStart());
    }
    protected IEnumerator DelayStart()
    {
#if !UNITY_EDITOR
        while (ARFace == null) 
        {
            ARFace = FindObjectOfType<ARFace>();
            yield return null;
        }
        faceRenderer = ARFace.GetComponent<Renderer>();
        //Tell the player to stand further back
        OnSpawnNextQuestion?.Invoke("Move away from the camera");

        //Only start the game once the player is far enough away from the camera
        float faceScreenSize = GetScreenSpaceDiagonal(faceRenderer.bounds);
        while (ARFace == null || faceScreenSize <= 0 || GetScreenSpaceDiagonal(faceRenderer.bounds) > 500) 
        {
            if(ARFace == null)
            {
                ARFace = FindObjectOfType<ARFace>();
                if(ARFace != null){
                    faceRenderer = ARFace.GetComponent<Renderer>();
                    yield return null;
                }
            }
            Debug.Log("faceRenderer: "+faceRenderer);
            yield return null;
        }
#endif
        OnSpawnNextQuestion?.Invoke("");
        coroutineComplete = true;
        OnDisplayLargeText?.Invoke(3.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(2.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(1.ToString());
        yield return new WaitForSeconds(1f);

        started = true;
        SpawnAllAnswers();
    }
    protected float GetScreenSpaceDiagonal(Bounds bounds)
    {
        Vector3 min = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 max = Camera.main.WorldToScreenPoint(bounds.max);
        return (max - min).magnitude;
    }
    protected void UpdateTime()
    {
        //The code below searches for a face in the scene
#if !UNITY_EDITOR
        if (ARFace == null && started) 
        {
            //started = false;
            ARFace = FindObjectOfType<ARFace>();
            if(ARFace != null) 
            {
                faceRenderer = ARFace.GetComponent<Renderer>();
                started = true;
            }
        }

        if (ARFace == null && coroutineComplete)
        {
            ARFace = FindObjectOfType<ARFace>();
            if(ARFace != null) 
            {
                faceRenderer = ARFace.GetComponent<Renderer>();
                started = true;
            }
        }
#endif
        if (!started) return;
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            OnTimeUp();
        }
        float roundedRemainingTime = (float)Math.Round(remainingTime, 2);
        OnUpdateTimeRemaining?.Invoke(roundedRemainingTime.ToString());
    }
    protected abstract void SpawnAllAnswers();
    protected abstract void DestroyPreviousAnswers();
    protected abstract void OnTimeUp();
}
public class TrueFalseQuestionSpawner : QuestionSpawner
{
    public List<TrueFalseQuestionAnswer> questions;
    public Action<int> OnUpdateScore;
    public Action OnGameStarted;
    public TrueFalseRegion trueRegion;
    public TrueFalseRegion falseRegion;
    public bool currentCorrectAnswer;

    private int currentQuestionIndex;
    private TrueFalseCollisionDetection tfCollisionDetection;
    protected void Awake()
    {
        trueRegion.IsTrue = true;
        falseRegion.IsTrue = false;
    }
    protected override void Start() 
    {
        StartCoroutine(TryFindCollisionDetection());

    }
    IEnumerator TryFindCollisionDetection() 
    {
        while (tfCollisionDetection == null) 
        {
            tfCollisionDetection = FindObjectOfType<TrueFalseCollisionDetection>();
            yield return null;
        }
        tfCollisionDetection.OnSelectAnswer += CheckIfCorrect;
        currentCorrectAnswer = questions[currentQuestionIndex].correctAnswer;
        remainingTime = GameTime;
        base.Start();
    }
    protected override void DestroyPreviousAnswers()
    {
        OnDisplayLargeText?.Invoke("Done!");
        OnSpawnNextQuestion?.Invoke("");
        OnUpdateTimeRemaining?.Invoke("0.00");
        OnFinished?.Invoke();
    }

    protected override void OnTimeUp()
    {
        DestroyPreviousAnswers();
        started = false;
    }

    protected void CheckIfCorrect(bool isTrue) 
    {
        Debug.Log("checking if question is correct");
        int val = (isTrue == questions[currentQuestionIndex].correctAnswer) ? 1 : -1;
        OnUpdateScore?.Invoke(val);
        currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
        SpawnAllAnswers();
    }

    protected override void SpawnAllAnswers()
    {   
        OnSpawnNextQuestion?.Invoke(questions[currentQuestionIndex].question);
        OnGameStarted?.Invoke();
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("The status of started is: " + started);
        base.UpdateTime();
        if (!started) return;
        if (tfCollisionDetection == null) 
        {
            tfCollisionDetection = FindObjectOfType<TrueFalseCollisionDetection>();
            if (tfCollisionDetection != null) 
            {
                OnGameStarted?.Invoke();
                tfCollisionDetection.OnSelectAnswer -= CheckIfCorrect;
                tfCollisionDetection.OnSelectAnswer += CheckIfCorrect;
            }
        }
        currentCorrectAnswer = questions[currentQuestionIndex].correctAnswer;

    }
}
