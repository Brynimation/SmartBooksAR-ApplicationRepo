using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlaceValueUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text scoreText;
    [Header("Large Text")]
    [SerializeField] TMP_Text largeText;
    [SerializeField] CanvasGroup largeTextCG;
    [Header("Flash Screen")]
    [SerializeField] float flashScreenTime;
    [SerializeField] CanvasGroup flashScreenCG;
    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    PlaceValueController placeValueController;

    int score;
    void Awake()
    {
        placeValueController= FindObjectOfType<PlaceValueController>();
        placeValueController.OnSpawnNextQuestion += SetQuestionText;
        placeValueController.OnSpawnLargeText += SetLargeText;
        placeValueController.OnSelectAnswer += SetScoreText;
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Quit()
    {
        SceneManager.LoadScene(2);
    }
    void SetLargeText(string text) 
    {
        largeText.SetText(text);
        StartCoroutine(FadeCG(largeTextCG, flashScreenTime));
    }
    void SetQuestionText(string question) 
    {
        questionText.SetText(question);
        StartFlashScreenFade();
    }
    void SetScoreText(int scoreChange) 
    {
        score += scoreChange;
        scoreText.SetText("Score: " + score);
    }

    void StartFlashScreenFade() 
    {
        StartCoroutine(FadeCG(flashScreenCG, flashScreenTime));
    }
    IEnumerator FadeCG(CanvasGroup cg, float fadeTime) 
    {
        cg.gameObject.SetActive(true);
        float timeElapsed = 0f;
        while (timeElapsed < fadeTime) 
        {
            cg.alpha = 1f - timeElapsed / fadeTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        cg.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
