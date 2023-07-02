using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WallUI : MonoBehaviour
{
    [Header("Values")]
    public int targetValue;
    public int currentValue;

    [Header("Large text")]
    [SerializeField] TMP_Text messageText;
    [SerializeField] TMP_Text LargeText;
    [SerializeField] CanvasGroup LargeTextCG;
    [SerializeField] float FadeInTime = 1f;


    [Header("Text")]
    [SerializeField] TMP_Text targetValueText;
    [SerializeField] TMP_Text currentValueText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] float colourChangeTime;
    [SerializeField] Color correctColour;
    [SerializeField] Color incorrectColour;
    [SerializeField] Color changeColour;

    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    [Header("Trophy")]
    [SerializeField] CanvasGroup fadeImage;
    [SerializeField] TMP_Text finishedText;
    [SerializeField] GameObject scoreTrophy;
    [SerializeField] GameObject congratulationsTrophy;
    [SerializeField] TMP_Text congratulationsScore;



    WallCollisionDetection wallSelection;
    WallGenerator wallGenerator;
    float remainingTimeToFade = 0f;
    int score;


    void Awake()
    {
        wallSelection = FindObjectOfType<WallCollisionDetection>();
        wallGenerator = FindObjectOfType<WallGenerator>();

        wallGenerator.OnSpawnNewValues += UpdateValues;
        wallGenerator.OnDisplayMessage += DisplayMessage;
        wallGenerator.OnDisplayLargeText += DisplayLargeText;
        WallPiece.OnPieceSelected += UpdateCurrentValue;

        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);

        scoreText.SetText("0");
        finishedText.SetText("");
    }

    void Restart() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Quit() 
    {
        SceneManager.LoadScene(2);
    }
    void UpdateValues(int current, int target, bool isSecondAttempt) 
    {
        if (target == 0 && current == 0) 
        {
            StartCoroutine(FadeImage(1f, fadeImage, false));
            targetValueText.SetText("");
            finishedText.SetText("Congratulations!");
            StartCoroutine(ChangeColour(colourChangeTime, targetValueText, targetValueText.color, changeColour));
            currentValueText.SetText("");
            scoreTrophy.SetActive(false);
            congratulationsTrophy.SetActive(true);
            congratulationsScore.SetText(score.ToString());
            return;
        }
        targetValue = target;
        currentValue = current;

        targetValueText.SetText(targetValue.ToString());
        StartCoroutine(ChangeColour(colourChangeTime, targetValueText, targetValueText.color, changeColour));
        currentValueText.SetText(currentValue.ToString());
        StartCoroutine(ChangeColour(colourChangeTime, currentValueText, currentValueText.color, changeColour));
    }
    void DisplayMessage(string msg) 
    {
        messageText.SetText(msg);
    }
    void DisplayLargeText(string msg) 
    {
        LargeText.SetText(msg);
        StartCoroutine(FadeImage(FadeInTime, LargeTextCG, true));
    }
    private void UpdateCurrentValue(int val, bool isSecondAttempt) 
    {
        currentValue += val;
        currentValueText.SetText(currentValue.ToString());
        StartCoroutine(ChangeColour(colourChangeTime, currentValueText, currentValueText.color, changeColour));
        if (currentValue == targetValue)
        {
            score = (isSecondAttempt) ? score + 2 : score + 1;
        }
        else {
            score -= 1;
        }
        
        Color flashColour = currentValue == targetValue ? correctColour : incorrectColour;
        StartCoroutine(ChangeColour(colourChangeTime, scoreText, scoreText.color, flashColour));
        scoreText.SetText(score.ToString());
    }
    //TO DO: As this FadeImage code is common among UI classes, create a parent class that all UI classes inherit from
    IEnumerator FadeImage(float fadeInTime, CanvasGroup cg, bool fadeIn)
    {
        cg.gameObject.SetActive(true);
        while (remainingTimeToFade < fadeInTime)
        {
            float alpha = fadeIn ? remainingTimeToFade / fadeInTime : 1f - remainingTimeToFade / fadeInTime;
            cg.alpha = alpha;
            remainingTimeToFade += Time.deltaTime;
            yield return null;
        }
        remainingTimeToFade = 0f;
        cg.gameObject.SetActive(false);

    }
    IEnumerator ChangeColour(float changeTime, TMP_Text text, Color originalColour, Color flashColour) 
    {
        text.color = flashColour;
        float elapsedTime = 0f;
        while (elapsedTime < changeTime) 
        {
            text.color = Color.Lerp(flashColour, originalColour, elapsedTime / changeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
