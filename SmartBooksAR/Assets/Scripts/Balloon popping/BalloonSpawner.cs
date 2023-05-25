using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public struct QuestionAnswer
{
    public string question;

    public List<string> answers;
    public List<int> correctAnswerIndices;
}
public class BalloonSpawner : MonoBehaviour
{
    public float timePerQuestion;
    public List<QuestionAnswer> questionAnswers;
    public Balloon balloonPrefab;

    public Action<string> OnSpawnNextQuestion; //Event - when invoked, it tells the UI to display the next question
    public Action<string> OnDisplayLargeText; //Event - when invoked, it tells the UI to display large text
    public Action<string> OnUpdateTimeRemaining;//Event - when invoked, it tells the UI to update the time remaining text.

    private float remainingTime;
    private bool started = false;
    private bool coroutineComplete = false;
    private ARFace ARFace;
    private Renderer faceRenderer;
    int currentQuestionIndex;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    void Start()
    {
        remainingTime = timePerQuestion;
        StartCoroutine(DelayStart());

    }

    //Determines the screen-space size of the bounding volume of the given renderer.
    //This function is used to determine how much space the user's face occupies on screen, and gets them to stand back if they are too close.
    private float GetScreenSpaceDiagonal(Bounds bounds) 
    {
        Vector3 min = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 max = Camera.main.WorldToScreenPoint(bounds.max);
        return (max - min).magnitude;
    }

    IEnumerator DelayStart()
    {
        //The code below searches for a face in the scene
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
        coroutineComplete = true;
        OnDisplayLargeText?.Invoke(3.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(2.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(1.ToString());
        yield return new WaitForSeconds(1f);

        started = true;
        OnSpawnNextQuestion?.Invoke(questionAnswers[currentQuestionIndex].question);
        OnUpdateTimeRemaining?.Invoke(timePerQuestion.ToString());
        SpawnAllAnswers();

    }

    //TO DO: This code is quite similar among all classes used to spawn answers. It might be worthwhile moving this sort of functionality to a base class that all answer spawners must inherit from.
    void SpawnAllAnswers() 
    {

        //If we've reached the end of the list
        if (currentQuestionIndex == questionAnswers.Count) 
        {
            OnSpawnNextQuestion?.Invoke("Finished!");
            return;
        }

        List<string> answers = questionAnswers[currentQuestionIndex].answers;
        List<int> correctIndices = questionAnswers[currentQuestionIndex].correctAnswerIndices;
        //Spawn balloons containing the answers
        for (int i = 0; i < answers.Count; i++)
        {
            string answer = answers[i];
            Vector2 randomViewportPos = new Vector2(UnityEngine.Random.Range(0.1f, 0.9f), UnityEngine.Random.Range(0.1f, 0.9f));
            SpawnBalloonAnswer(randomViewportPos, answer, i, correctIndices);
        }

        //Destroy all balloons created for the previous question


        //Send event to UI to change the question
        OnSpawnNextQuestion?.Invoke(questionAnswers[currentQuestionIndex].question);
       
        //Move to next quesetion
        currentQuestionIndex++;
    }
    //Initialises a balloon with a starting position and text to display.
    void SpawnBalloonAnswer(Vector2 viewportPosition, string answerText, int curIndex, List<int> correctIndices) 
    {
        Balloon balloon = Instantiate(balloonPrefab, transform.position, Quaternion.identity);
        balloon.InitialiseAnswer(viewportPosition, answerText, curIndex, correctIndices, faceRenderer);
    }

    private void DestroyPreviousAnswers() 
    {
        Balloon[] balloons = FindObjectsOfType<Balloon>();
        foreach (Balloon balloon in balloons) 
        {
            Destroy(balloon.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //The code below searches for a face in the scene
#if !UNITY_EDITOR
        if (ARFace == null && started) 
        {
            started = false;
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
            remainingTime = timePerQuestion;
            DestroyPreviousAnswers();
            SpawnAllAnswers();
        }
        float roundedRemainingTime = (float) Math.Round(remainingTime, 2);
        OnUpdateTimeRemaining?.Invoke(roundedRemainingTime.ToString());
    }
}
