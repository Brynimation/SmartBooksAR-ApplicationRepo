using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BalloonUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text QuestionText;
    [SerializeField] TMP_Text TimerText;

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

    private int score = 0;
    private BalloonSpawner balloonSpawner;
    private float remainingTimeToFade;

    void Awake()
    {
        balloonSpawner = FindObjectOfType<BalloonSpawner>();

        //Subscribe to relevant events invoked by the BalloonSpawner.
        balloonSpawner.OnSpawnNextQuestion += UpdateQuestionText;
        balloonSpawner.OnUpdateTimeRemaining += UpdateTimeRemainingText;
        balloonSpawner.OnDisplayLargeText += DisplayLargeText;

        Balloon.OnDestroyBalloon += UpdateScoreText;
        QuestionText.SetText("");
        UpdateTimeRemainingText("");
        UpdateScoreText(0);
     }

    private void Start()
    {
        RestartButton.onClick.AddListener(Restart);
        QuitButton.onClick.AddListener(Quit);
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
        SceneManager.LoadScene(0);
    }
    private void Quit()
    {
        Application.Quit();
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

    }
    private void UpdateTimeRemainingText(string text) 
    {
        TimerText.SetText(text);
    }
    private void UpdateScoreText(int value) 
    {
        score += value;
        ScoreText.SetText("Score: "+score.ToString());
    }

}
