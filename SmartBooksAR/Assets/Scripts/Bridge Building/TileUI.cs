using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TileUI : MonoBehaviour
{
    [Header("Flash Screen")]
    [SerializeField] CanvasGroup flashScreen;

    [Header("Large text")]
    [SerializeField] TMP_Text messageText;
    [SerializeField] TMP_Text LargeText;
    [SerializeField] CanvasGroup LargeTextCG;
    [SerializeField] float FadeInTime = 1f;

    [Header("Target Representation")]
    [SerializeField] Image image;

    [Header("Timer")]
    [SerializeField] TMP_Text timerText;

    [Header("Buttons")]
    [SerializeField] Button QuitButton;
    [SerializeField] Button RestartButton;

    TileGenerator tileGenerator;
    TileCollisionDetection tileDetection;
    float remainingTimeToFade;
    bool findingTileDetection;

    int numCorrectSelected;
    IEnumerator GetTileDetection() 
    {
        findingTileDetection = true;
        while (tileDetection == null)
        {
            Debug.Log("In the loop!");
            tileDetection = FindObjectOfType<TileCollisionDetection>();
            yield return null;
        }
        Debug.Log("Tile detection found: " + tileDetection);
        tileDetection.OnUpdateTimeRemaining += UpdateTimeRemainingText;
        findingTileDetection = false;
    }
    void Start()
    {

        tileGenerator = FindObjectOfType<TileGenerator>();
        StartCoroutine(GetTileDetection());
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        //Subscribe to relevant events
        tileGenerator.OnSpawnNextQuestion += ChangeQuestionSprite;
        tileGenerator.OnDisplayMessage += DisplayMessage;
        tileGenerator.OnDisplayLargeText += DisplayLargeText;
        tileGenerator.OnAllCorrectSelected += ClearQuestion;

        RestartButton.onClick.AddListener(Restart);
        QuitButton.onClick.AddListener(Quit);

        LargeTextCG.gameObject.SetActive(false);

    }

    void Restart() 
    {
        SceneManager.LoadScene(1);
    }
    void Quit() 
    {
        Application.Quit();
    }

    //Called when the next question is invoked
    void ChangeQuestionSprite(Sprite sprite) 
    {
        StartCoroutine(FadeInImage(FadeInTime, flashScreen));
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        image.sprite = sprite;
    }
    //Called when all correct answers are selected
    void ClearQuestion() 
    {
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        image.sprite = null;
    }

    private void UpdateTimeRemainingText(float time)
    {
        string text = (time >= 0) ? time.ToString() : "";
        timerText.SetText(text);
    }
    //TO DO: As this FadeInImage code is common among UI classes, create a parent class that all UI classes inherit from
    IEnumerator FadeInImage(float fadeInTime, CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        while (remainingTimeToFade < fadeInTime)
        {
            cg.alpha = 1f - remainingTimeToFade/fadeInTime;
            remainingTimeToFade += Time.deltaTime;
            yield return null;
        }
        remainingTimeToFade = 0f;
        cg.gameObject.SetActive(false);

    }

    void DisplayMessage(string text) 
    {
        messageText.SetText(text);
    }
    void DisplayLargeText(string text) 
    {
        LargeText.SetText(text);
        StartCoroutine(FadeInImage(FadeInTime, LargeTextCG));
    }
    void Update()
    {
        if (tileDetection == null && !findingTileDetection) 
        {
            UpdateTimeRemainingText(0);
            StartCoroutine(GetTileDetection());
        }
    }
}
