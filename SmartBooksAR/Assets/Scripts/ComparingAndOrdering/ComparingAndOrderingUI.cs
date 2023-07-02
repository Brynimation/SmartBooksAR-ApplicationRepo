using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class ComparingAndOrderingUI : MonoBehaviour
{
    [Header("Flash screen")]
    [SerializeField] CanvasGroup flashScreenCG;
    
    [Header("Text")]
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text leftToFindText;

    [Header("Button")]
    [SerializeField] Button quitButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button testButton;

    [Header("Large Text")]
    [SerializeField] CanvasGroup largeTextCG;
    [SerializeField] TMP_Text largeText;

    [Header("Star")]
    [SerializeField] CollectedNumber collectedNumberPrefab;
    [SerializeField] GameObject starExplosion;
    [SerializeField] Transform startPositionT;
    int collectedNumberIndex;

    Canvas canvas;
    List<CollectedNumber> collectedNumbers;
    SpawnAnswersInWorld answerSpawner;
    WorldAnswerSelection answerSelector;
    bool firstCollected = true;
    private int score = 0;
    private int leftToFind;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
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

        scoreText.SetText("0");
        //testButton.onClick.AddListener(TestButton);

    }

    void TestButton() 
    {
        UpdateCollectedNumbers(true, Random.Range(0, 20));
    }

    //When a correct number is selected, the OnSelectAnswer static event is invoked. This tells the UI to display that number at the bottom of the screen
    private void UpdateCollectedNumbers(bool correct, int value)
    {
        if (!correct) return;
        leftToFind--;
        leftToFindText.SetText("Left to find: " + leftToFind.ToString());
        Vector3 spawnPos = startPositionT.position;
        if (firstCollected)
        {
            collectedNumbers = new List<CollectedNumber>();
            firstCollected = false;
        }
        else {
            Debug.Log("here!");
            Debug.Log(collectedNumberIndex);
            Debug.Log("collected numbers width: " + collectedNumbers[0].Width);
            spawnPos = collectedNumbers[collectedNumberIndex - 1].transform.position - (collectedNumbers[0].Width) * Vector3.right;
        }
        Debug.Log(collectedNumberIndex + ", " + collectedNumbers.Count);
        CollectedNumber collectedNum = Instantiate(collectedNumberPrefab, Vector3.zero, Quaternion.identity).GetComponent<CollectedNumber>();
        collectedNum.transform.SetParent(canvas.transform);
        collectedNum.transform.position = spawnPos;
        /*GameObject explosion = Instantiate(starExplosion, Vector3.zero, Quaternion.identity);
        explosion.transform.SetParent(canvas.transform);
        explosion.transform.position = spawnPos;*/
        collectedNum.SetNumberText(value.ToString());
        collectedNumbers.Add(collectedNum);
        collectedNumberIndex++;
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
    private void StartSpawnQuestion(string questionString, int totalCorrectAnswers) 
    {
        if(collectedNumbers != null)
        {
            foreach (CollectedNumber collectedNumber in collectedNumbers) 
            {
                Destroy(collectedNumber.gameObject);
            }
        }
        firstCollected = true;
        collectedNumberIndex = 0;
        leftToFind = totalCorrectAnswers;
        leftToFindText.SetText("Left to find: " + leftToFind.ToString());
        StartCoroutine(FadeIn(questionText, questionString, 2f, flashScreenCG));
    }


    private void UpdateScore(bool correct)
    {
        score = correct ? score + 1 : score - 1;
        scoreText.SetText(score.ToString());
    }
    private void Quit()
    {
        SceneManager.LoadScene(2);
    }
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
