using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuitPanel : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button cancelButton;
    [SerializeField] TMP_Text text;
    [SerializeField] string message;
    [SerializeField] int sceneToReturnTo;
    void Start()
    {
        text.SetText(message);
        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
        cancelButton.onClick.AddListener(No);
    }

    void Yes()
    {
        if (sceneToReturnTo == -1)
        {
            Application.Quit();
        }
        else {
            SceneManager.LoadScene(sceneToReturnTo);
        }
        
    }
    void No()
    {
        this.gameObject.SetActive(false);
    }
}
