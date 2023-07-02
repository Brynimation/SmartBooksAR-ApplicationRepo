using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;

public class BalloonUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] Canvas canvas;
    [Header("UI Text")]
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text QuestionText;
    [SerializeField] TMP_Text TimerText;
    [SerializeField] GameObject scoreTrophy;

    [Header("Win trophy")]
    [SerializeField] GameObject congratulationsTrophy;
    [SerializeField] TMP_Text congratulationsScore;

    [Header("Buttons")]
    [SerializeField] Button RestartButton;
    [SerializeField] Button QuitButton;

    [Header("Large text")]
    [SerializeField] TMP_Text LargeText;
    [SerializeField] CanvasGroup LargeTextCG;
    [SerializeField] float TextFadeInTime;

    [Header("Flash")]
    [SerializeField] CanvasGroup FlashCG;
    [SerializeField] float FlashFadeInTime;

    [Header("Star")]
    [SerializeField] CollectedNumber collectedNumberPrefab;
    [SerializeField] Transform startPositionT;

    private int score = 0;
    private BalloonSpawner balloonSpawner;
    private float remainingTimeToFade;
    private List<CollectedNumber> collectedNumbers;
    private bool firstCollected = true;
    private int collectedNumberIndex;
    void Awake()
    {
        balloonSpawner = FindObjectOfType<BalloonSpawner>();

        //Subscribe to relevant events invoked by the BalloonSpawner.
        balloonSpawner.OnSpawnNextQuestion += UpdateQuestionText;
        balloonSpawner.OnUpdateTimeRemaining += UpdateTimeRemainingText;
        balloonSpawner.OnDisplayLargeText += DisplayLargeText;
        balloonSpawner.OnGameEnd += OnGameEnd;

        Balloon.OnDestroyBalloon += UpdateScoreText;
        QuestionText.SetText("");
        UpdateTimeRemainingText("0");
        UpdateScoreText(0, "");
     }

    private void UpdateCollectedNumbers(string value)
    {
        Vector3 spawnPos = startPositionT.position;
        if (firstCollected)
        {
            collectedNumbers = new List<CollectedNumber>();
            firstCollected = false;
        }
        else
        {
            spawnPos = collectedNumbers[collectedNumberIndex - 1].transform.position - (collectedNumbers[0].Width) * Vector3.right;
        }
        Debug.Log(collectedNumberIndex + ", " + collectedNumbers.Count);
        CollectedNumber collectedNum = Instantiate(collectedNumberPrefab, Vector3.zero, Quaternion.identity).GetComponent<CollectedNumber>();
        collectedNum.transform.SetParent(canvas.transform);
        collectedNum.transform.position = spawnPos;
        /*GameObject explosion = Instantiate(starExplosion, Vector3.zero, Quaternion.identity);
        explosion.transform.SetParent(canvas.transform);
        explosion.transform.position = spawnPos;*/
        collectedNum.SetNumberText(value);
        collectedNumbers.Add(collectedNum);
        collectedNumberIndex++;
    }

    private void Start()
    {
        RestartButton.onClick.AddListener(Restart);
        QuitButton.onClick.AddListener(Quit);
    }

    void OnGameEnd() 
    {
        congratulationsTrophy.SetActive(true);
        congratulationsScore.SetText(score.ToString());
        scoreTrophy.SetActive(false);
    }
    void DisplayLargeText(string text) 
    {
        LargeText.SetText(text);
        StartCoroutine(FadeInImage(TextFadeInTime, LargeTextCG));
    }

    //TO DO: This Fading code is common to most of the different UI classes. Maybe the UIs could all inherit from a base UI class that contain this function definitio?
    IEnumerator FadeInImage(float fadeInTime, CanvasGroup cg) 
    {
        cg.gameObject.SetActive(true);
        Debug.Log(LargeTextCG.isActiveAndEnabled);
        while (remainingTimeToFade < fadeInTime) 
        {
            cg.alpha = fadeInTime - remainingTimeToFade;
            remainingTimeToFade += Time.deltaTime;
            yield return null;
        }
        remainingTimeToFade = 0f;
        cg.gameObject.SetActive(false);

    }

    private void Restart() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Quit()
    {
        SceneManager.LoadScene(2);
    }

    private void ClearAllCollectedNumbers() 
    {
        if (collectedNumbers != null)
        {
            foreach (CollectedNumber collectedNumber in collectedNumbers)
            {
                Destroy(collectedNumber.gameObject);
            }
        }
        firstCollected = true;
        collectedNumberIndex = 0;
    }
    private void UpdateQuestionText(string text) 
    {
        Debug.Log(text);
        QuestionText.SetText(text);

        StartCoroutine(FadeInImage(FlashFadeInTime, FlashCG));
        if (text == "Finished!") 
        {
            balloonSpawner.OnSpawnNextQuestion -= UpdateQuestionText;
            balloonSpawner.OnUpdateTimeRemaining -= UpdateTimeRemainingText;
            balloonSpawner.OnDisplayLargeText -= DisplayLargeText;

            Balloon.OnDestroyBalloon -= UpdateScoreText;
            UpdateTimeRemainingText("");
        }
        ClearAllCollectedNumbers();
    }
    private void UpdateTimeRemainingText(string text) 
    {
        TimerText.SetText(text);
    }
    private void UpdateScoreText(int scoreChange, string value) 
    {
        Debug.Log("updating score text");
        score += scoreChange;
        ScoreText.SetText(score.ToString());
        if (scoreChange >= 1) 
        {
            UpdateCollectedNumbers(value);
        }
    }

}
