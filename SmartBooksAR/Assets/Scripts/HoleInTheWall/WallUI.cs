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
    }

    void Restart() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Quit() 
    {
        SceneManager.LoadScene(2);
    }
    void UpdateValues(int target, int current) 
    {
        if (target == 0 && current == 0) 
        {
            targetValueText.SetText("Finished!");
            StartCoroutine(ChangeColour(colourChangeTime, targetValueText, targetValueText.color, changeColour));
            currentValueText.SetText("");
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
        StartCoroutine(FadeInImage(FadeInTime, LargeTextCG));
    }
    private void UpdateCurrentValue(int val) 
    {
        currentValue += val;
        currentValueText.SetText(currentValue.ToString());
        StartCoroutine(ChangeColour(colourChangeTime, currentValueText, currentValueText.color, changeColour));
        score = (currentValue == targetValue) ? score + 1 : score - 1;
        Color flashColour = currentValue == targetValue ? correctColour : incorrectColour;
        StartCoroutine(ChangeColour(colourChangeTime, scoreText, scoreText.color, flashColour));
        scoreText.SetText(score.ToString());
    }
    //TO DO: As this FadeInImage code is common among UI classes, create a parent class that all UI classes inherit from
    IEnumerator FadeInImage(float fadeInTime, CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        while (remainingTimeToFade < fadeInTime)
        {
            cg.alpha = remainingTimeToFade / fadeInTime;
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
