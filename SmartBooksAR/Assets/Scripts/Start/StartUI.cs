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
    [SerializeField] Button balloonButton;
    [SerializeField] Button bridgeButton;
    [SerializeField] Button comparingAndOrderingButton;
    [SerializeField] float transitionTime;
    [SerializeField] CanvasGroup canvasGroup;
    private float timeElapsed;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    void Start()
    {
        balloonButton.onClick.AddListener(StartBalloonScene);
        bridgeButton.onClick.AddListener(StartBridgeScene);
        comparingAndOrderingButton.onClick.AddListener(StartComparingAndOrderingScene);
    }
    void StartBalloonScene() 
    {
        StartCoroutine(ChangeSceneTransition(1));
    }
    void StartBridgeScene() 
    {
        StartCoroutine(ChangeSceneTransition(2));
    }
    void StartComparingAndOrderingScene() 
    {
        StartCoroutine(ChangeSceneTransition(3));
    }

    IEnumerator ChangeSceneTransition(int sceneIndex) 
    {
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
