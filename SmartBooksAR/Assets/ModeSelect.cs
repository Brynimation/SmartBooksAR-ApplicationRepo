using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class ModeSelect : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button gamesButton;
    [SerializeField] Button tutorialsButton;
    [SerializeField] Button homeButton;

    [Header("Fading")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float transitionTime;

    [SerializeField] GameObject quitPanel;

    private void Awake()
    {
        gamesButton.onClick.AddListener(StartGames);
        homeButton.onClick.AddListener(GoHome);
    }
    private void Start()
    {

    }
    void StartGames() 
    {
        StartCoroutine(ChangeSceneTransition(2));
    }
    void GoHome() 
    {
        quitPanel.SetActive(true);
    }

    IEnumerator ChangeSceneTransition(int sceneIndex)
    {
        float timeElapsed = 0f;
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        while (timeElapsed < transitionTime)
        {
            canvasGroup.alpha = timeElapsed / transitionTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneIndex);
    }
}
