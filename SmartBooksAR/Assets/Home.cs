using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] float sceneTransitionTime;
    [SerializeField] CanvasGroup flashScreenCG;
    void Start()
    {
        startButton.onClick.AddListener(StartPage);
    }
    void StartPage() 
    {
        StartCoroutine(StartPageCoroutine());
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
