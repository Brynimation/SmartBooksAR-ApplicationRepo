using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrueFalseUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text timerText;

    [Header("Large Text")]
    [SerializeField] TMP_Text largeText;
    [SerializeField] CanvasGroup largeTextCG;
    [SerializeField] float textFadeInTime;

    [Header("Buttons")]
    [SerializeField] Button RestartButton;
    [SerializeField] Button QuitButton;
    private int score;
    private TrueFalseQuestionSpawner tfQuestionSpawner;

    private void Awake()
    {
        scoreText.SetText("Score: 0");
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    void Start()
    {
        tfQuestionSpawner = FindObjectOfType<TrueFalseQuestionSpawner>();
        tfQuestionSpawner.OnUpdateScore += UpdateScore;
        tfQuestionSpawner.OnUpdateTimeRemaining += UpdateTimeRemaining;
        tfQuestionSpawner.OnDisplayLargeText += DisplayLargeText;
        tfQuestionSpawner.OnSpawnNextQuestion += UpdateQuestion;
        RestartButton.onClick.AddListener(Restart);
        QuitButton.onClick.AddListener(Quit);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }
    private void Quit()
    {
        Application.Quit();
    }


    void UpdateScore(int val) 
    {
        Debug.Log("updating score text");
        score += val;
        scoreText.SetText("Score: " + score.ToString());
    }
    void DisplayLargeText(string text)
    {
        largeText.SetText(text);
        StartCoroutine(FadeInImage(textFadeInTime, largeTextCG));
    }

    //TO DO: This Fading code is common to most of the different UI classes. Maybe the UIs could all inherit from a base UI class that contain this function definitio?
    IEnumerator FadeInImage(float fadeInTime, CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        float timeElapsed = 0f;
        while (timeElapsed < fadeInTime)
        {
            cg.alpha = fadeInTime - timeElapsed/fadeInTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        timeElapsed = 0f;
        cg.gameObject.SetActive(false);

    }
    void UpdateTimeRemaining(string time) 
    {
        timerText.SetText(time);
    }
    void UpdateQuestion(string text) 
    {
        questionText.SetText(text);
    }
    void Update()
    {
        
    }
}
