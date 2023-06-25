using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

//This is just a basic navigation page. There is a button to each implemented game
public class StartUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button balloonButton;
    [SerializeField] Button comparingAndOrderingButton;
    [SerializeField] Button holeInTheWallButton;
    [SerializeField] Button placeValueButton;
    [SerializeField] Button trueFalseButton;
    [SerializeField] Button homeButton;

    [Header("Scene Transition")]
    [SerializeField] float transitionTime;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] GameObject quitPanel;
    
    private float timeElapsed;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    void Start()
    {
        balloonButton.onClick.AddListener(StartBalloonScene);
        //bridgeButton.onClick.AddListener(StartBridgeScene);
        comparingAndOrderingButton.onClick.AddListener(StartComparingAndOrderingScene);
        holeInTheWallButton.onClick.AddListener(StartHoleInTheWallScene);
        placeValueButton.onClick.AddListener(StartPlaceValueScene);
        trueFalseButton.onClick.AddListener(StartTrueFalseScene);
        homeButton.onClick.AddListener(() => quitPanel.gameObject.SetActive(true));
    }
    void StartBalloonScene() 
    {
        StartCoroutine(ChangeSceneTransition(3));
    }
    void StartBridgeScene() 
    {
        StartCoroutine(ChangeSceneTransition(2));
    }
    void StartComparingAndOrderingScene() 
    {
        StartCoroutine(ChangeSceneTransition(4));
    }
    void StartHoleInTheWallScene() 
    {
        StartCoroutine(ChangeSceneTransition(5));
    }
    void StartPlaceValueScene() 
    {
        StartCoroutine(ChangeSceneTransition(6));
    }
    void StartTrueFalseScene() 
    {
        StartCoroutine(ChangeSceneTransition(7));
    }

    IEnumerator ChangeSceneTransition(int sceneIndex) 
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
