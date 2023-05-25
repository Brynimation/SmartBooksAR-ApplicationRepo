using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ComparingAndOrderingUI : MonoBehaviour
{
    [Header("Flash screen")]
    [SerializeField] CanvasGroup flashScreenCG;
    
    [Header("Text")]
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text collectedAnswersText;
    [SerializeField] TMP_Text scoreText;

    [Header("Button")]
    [SerializeField] Button quitButton;
    [SerializeField] Button restartButton;

    [Header("Large Text")]
    [SerializeField] CanvasGroup largeTextCG;
    [SerializeField] TMP_Text largeText;


    SpawnAnswersInWorld answerSpawner;
    WorldAnswerSelection answerSelector;
    bool firstCollected = false;
    private int score = 0;
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        largeTextCG.gameObject.SetActive(false);

        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);

        //Subscribe to relevant events
        answerSpawner = FindObjectOfType<SpawnAnswersInWorld>();
        answerSpawner.OnSpawnNextQuestion += StartSpawnQuestion;
        answerSpawner.OnDisplayLargeText += DisplayLargeText;
        answerSelector = FindObjectOfType<WorldAnswerSelection>();
        answerSelector.OnSelectAnswer += UpdateScore;
        AnswerInWorld.OnSelectAnswer += UpdateCollectedNumbers;

        scoreText.SetText("Score: 0");
        collectedAnswersText.SetText("Collected:");

    }

    //When a correct number is selected, the OnSelectAnswer static event is invoked. This tells the UI to display that number at the bottom of the screen
    private void UpdateCollectedNumbers(bool correct, int value) 
    {
        if (!correct) return;
        string collectedAnswers = collectedAnswersText.text;
        if (!firstCollected)
        {
            collectedAnswers += " ";
            firstCollected = true;
        }
        else {
            collectedAnswers += ", ";
        }
        collectedAnswers += value.ToString();
        collectedAnswersText.SetText(collectedAnswers);
    }

    private void DisplayLargeText(string textString, float fadeTime) 
    {
        StartCoroutine(FadeIn(largeText, textString, fadeTime, largeTextCG));
    }

    IEnumerator FadeIn(TMP_Text text, string textString, float fadeTime, CanvasGroup cg) 
    {
        cg.gameObject.SetActive(true);
        text.SetText(textString);
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            cg.alpha = 1f - elapsedTime / fadeTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cg.gameObject.SetActive(false);
    }
    private void StartSpawnQuestion(string questionString) 
    {
        StartCoroutine(FadeIn(questionText, questionString, 2f, flashScreenCG));
    }


    private void UpdateScore(bool correct)
    {
        score = correct ? score + 1 : score - 1;
        scoreText.SetText("Score: " + score.ToString());
    }
    private void Quit()
    {
        Application.Quit();
    }
    private void Restart()
    {
        SceneManager.LoadScene(3);
    }
}
