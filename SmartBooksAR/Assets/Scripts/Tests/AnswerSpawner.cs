using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;



public class AnswerSpawner : MonoBehaviour
{
    public Action<string> OnStartNextQuestion;
    ARFaceManager manager;
    [Header("Visuals")]
    [SerializeField] Vector2 minMaxTimeOnScreen;
    [SerializeField] Vector2 minMaxPlatformLength;
    [SerializeField] GameObject answerPrefab;
    [SerializeField] ARFace face;
    [SerializeField] Renderer mRenderer;
    [SerializeField] GameObject spherePrefab;
    GameObject instantiatedSphere;

    [Header("Questions")]
    [SerializeField] List<QuestionAnswer> questionAnswers;
    int numQuestions;
    IEnumerator FindFace() 
    {


        //Debug.Log("here we go!");
        face = FindObjectOfType<ARFace>();
        //Debug.Log("face: "+face);
        while (face == null) 
        {
            face = FindObjectOfType<ARFace>();
            yield return null;
        }
        mRenderer = face.gameObject.GetComponent<Renderer>();
        if (instantiatedSphere == null) 
        {
            //Debug.Log("instantiating new sphere");
            instantiatedSphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
        }
        
        //Debug.Log("renderer: " + mRenderer.name + "face: " + face.name + "instantiated sphere: " + target.name);
    }
    void Start()
    {
        numQuestions = questionAnswers.Count;
        manager = FindObjectOfType<ARFaceManager>();
        //manager.facesChanged += ManageEvent;
        MovingAnswer.OnSpawnNextQuestion += SpawnNextQuestion;
        SpawnNextQuestion();
        StartCoroutine(FindFace());
    }
    void ManageEvent(ARFacesChangedEventArgs e) 
    {
        List<ARFace> added = e.added;
        List<ARFace> updated = e.updated;
        List<ARFace> removed = e.removed;

        if (removed.Count > 0) 
        {
            Debug.Log("Removed count: " + removed.Count);
            Debug.Log("starting the coroutine!");
            StartCoroutine(FindFace());
        }

    }

    // Update is called once per frame
    IEnumerator SpawnNextAnswer(int numAnswersToQuestion, QuestionAnswer currentQuestion) 
    {
        for (int i = 0; i < numAnswersToQuestion; i++)
        {
            bool isCorrect = currentQuestion.correctAnswerIndices.Contains(i);
            MovingAnswer currentAnswer = Instantiate(answerPrefab, transform.position, Quaternion.identity).GetComponent<MovingAnswer>();
            float viewportX = Mathf.Lerp(0.25f, 0.75f, (float)i / (numAnswersToQuestion - 1f));
            currentAnswer.SetAnswerProperties(minMaxTimeOnScreen, minMaxPlatformLength, isCorrect, viewportX, currentQuestion.answers[i], numAnswersToQuestion);
            yield return new WaitForSeconds(2f);
        }
    }
    void SpawnNextQuestion()
    {
        Debug.Log(numQuestions);
        if (numQuestions <= 0) return;
        numQuestions--;
        QuestionAnswer currentQuestion = questionAnswers[questionAnswers.Count - numQuestions - 1];
        OnStartNextQuestion?.Invoke(currentQuestion.question);
        int numAnswersToQuestion = currentQuestion.answers.Count;
        StartCoroutine(SpawnNextAnswer(numAnswersToQuestion, currentQuestion));

    }
    void Update()
    {
        //Debug.Log("face: " + face + "renderer: " + mRenderer + "target: " + target);
        if (face != null && mRenderer != null && instantiatedSphere != null)
        {
            instantiatedSphere.transform.position = mRenderer.bounds.center;
            instantiatedSphere.transform.localScale = mRenderer.bounds.extents / 2f;
        }
        else {
            if(face == null) face = FindObjectOfType<ARFace>();
            if (face != null) 
            {
                mRenderer = face.gameObject.GetComponent<Renderer>();
            }
            if (instantiatedSphere != null)
            {
                instantiatedSphere.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(2f, 0.5f));
                instantiatedSphere.transform.localScale = Vector3.one / 10f;
            }

        }
    }
}
