using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnAnswersInWorld : MonoBehaviour
{

    //Events
    public Action<string> OnSpawnNextQuestion;
    public Action<string, float> OnDisplayLargeText;
    public Action OnAllCorrectAnswersSelected;

    [SerializeField] AnswerInWorld answerPrefab;
    [SerializeField] List<QuestionAnswer> questionAnswers;
    [SerializeField] float spawnRadius;

    private int currentQuestionIndex;
    private int totalCorrectAnswers;
    private int selectedCorrectAnswers;
    private void Awake()
    {
        AnswerInWorld.OnSelectAnswer += SelectAnswer;
    }
    void Start()
    {
        StartCoroutine(DelayStart());
    }

    private void SelectAnswer(bool correct, int value) 
    {
        //If the user has collected all correct answers, then the next question is displayed and the correct answers are spawned.
        selectedCorrectAnswers = correct ? selectedCorrectAnswers + 1 : selectedCorrectAnswers;
        if (selectedCorrectAnswers == totalCorrectAnswers) 
        {
            StartCoroutine(SpawnAllAnswersCoroutine());
        }
    }
    IEnumerator DelayStart()
    {
        for (int i = 0; i < 3; i++) 
        {
            OnDisplayLargeText?.Invoke((3 - i).ToString(), 1f);
            yield return new WaitForSeconds(1f);
        }
        SpawnAllAnswers();
    }
    //TO DO: This is similar to the code in TileGenerator and BalloonSpawner. Is it possible to make this a generic function in a separate class?
    void SpawnAllAnswers()
    {

        //If we've reached the end of the list
        AnswerInWorld[] previousAnswers = FindObjectsOfType<AnswerInWorld>();
        foreach (AnswerInWorld answer in previousAnswers)
        {
            Destroy(answer.gameObject);
        }
        List<string> answers = questionAnswers[currentQuestionIndex].answers;
        List<int> correctIndices = questionAnswers[currentQuestionIndex].correctAnswerIndices;

        totalCorrectAnswers = correctIndices.Count;
        selectedCorrectAnswers = 0;
        //Spawn the answers
        for (int i = 0; i < answers.Count; i++)
        {
            SpawnAnswerInWorld(answers[i], i, correctIndices);
        }

        //Send event to UI to change the question
        OnSpawnNextQuestion?.Invoke(questionAnswers[currentQuestionIndex].question);

        //Move to next quesetion
        currentQuestionIndex++;
    }

    //Spawn all answers within a sphere of radius spawnRadius  around the origin (the original position of the user's phone)
    void SpawnAnswerInWorld(string answer, int index, List<int> correctIndices)
    {
        Vector3 pos = UnityEngine.Random.insideUnitSphere * spawnRadius;
        AnswerInWorld answerInWorld = Instantiate(answerPrefab, pos, Quaternion.identity);
        answerInWorld.gameObject.AddComponent<ARAnchor>();
        bool correct = correctIndices.Contains(index);
        answerInWorld.InitialiseAnswer(answer, correct);
    }
    IEnumerator SpawnAllAnswersCoroutine()
    {
        OnDisplayLargeText?.Invoke("Well done!", 2.5f);
        yield return new WaitForSeconds(2.5f);
        OnDisplayLargeText?.Invoke("All correct answers selected!", 2.5f);
        yield return new WaitForSeconds(2.5f);
        if (currentQuestionIndex == questionAnswers.Count)
        {
            OnSpawnNextQuestion?.Invoke("Finished!");
            yield return null;
        }
        else
        {
            OnDisplayLargeText?.Invoke("Next question in...", 1.5f);
            yield return new WaitForSeconds(1.5f);
            OnDisplayLargeText?.Invoke("3", 1f);
            yield return new WaitForSeconds(1f);
            OnDisplayLargeText?.Invoke("2", 1f);
            yield return new WaitForSeconds(1f);
            OnDisplayLargeText?.Invoke("1", 1f);
            yield return new WaitForSeconds(1f);
            SpawnAllAnswers();
        }

    }

    /*void SpawnAllAnswers()
    {
        AnswerInWorld[] previousAnswers = FindObjectsOfType<AnswerInWorld>();
        foreach (AnswerInWorld answer in previousAnswers)
        {
            Destroy(answer.gameObject);
        }
        List<string> answers = questionAnswers[currentQuestionIndex].answers;
        List<int> correctIndices = questionAnswers[currentQuestionIndex].correctAnswerIndices;

        totalCorrectAnswers = correctIndices.Count;
        selectedCorrectAnswers = 0;
        int index = 0;
        foreach (ARPlane plane in planeManager.trackables) 
        {
            Debug.Log(plane);
            if (index >= answers.Count) break;
            SpawnAnswerInWorld(answers[index], index, correctIndices, plane.center);
            index++;
        }

        //Send event to UI to change the question
        OnSpawnNextQuestion?.Invoke(questionAnswers[currentQuestionIndex].question);

        //Move to next quesetion
        currentQuestionIndex++;
    }
    void SpawnAnswerInWorld(string answer, int index, List<int> correctIndices, Vector3 pos) 
    {
        AnswerInWorld answerInWorld = Instantiate(answerPrefab, pos, Quaternion.identity);
        answerInWorld.gameObject.AddComponent<ARAnchor>();
        bool correct = correctIndices.Contains(index);
        answerInWorld.InitialiseAnswer(answer, correct);
    }*/


}
