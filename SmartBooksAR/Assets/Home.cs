using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Home : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button settingsButton;

    [SerializeField] GameObject quitPanel;
    [SerializeField] float sceneTransitionTime;
    [SerializeField] CanvasGroup flashScreenCG;
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        startButton.onClick.AddListener(StartPage);
        quitButton.onClick.AddListener(OnQuit);
    }
    void StartPage() 
    {
        StartCoroutine(StartPageCoroutine());
    }
    private void OnQuit()
    {
        quitPanel.gameObject.SetActive(true);
    }
    IEnumerator StartPageCoroutine() 
    {
        float timeElapsed = 0f;
        flashScreenCG.gameObject.SetActive(true);
        while (timeElapsed < sceneTransitionTime) 
        {
            flashScreenCG.alpha = timeElapsed / sceneTransitionTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(1);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
