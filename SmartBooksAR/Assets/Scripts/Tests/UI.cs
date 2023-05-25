using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text questionText;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    private int score;
    CollisionDetection collisionDetection;
    AnswerSpawner answerSpawner;
    private void SetScoreText(int score) 
    {
        scoreText.SetText("Score: " + score.ToString());
    }

    private void SetQuestionText(string question) 
    {
        questionText.SetText(question);
    }

    void Quit() 
    {
        Application.Quit();
    }
    void Restart() 
    {
        SceneManager.LoadScene(0);
    }
    void Awake() 
    {
        score = 0;
        scoreText.SetText("Score: 0");
    }

    IEnumerator TryGetCollisionDetection() 
    {
        while (collisionDetection == null) 
        {
            collisionDetection = FindObjectOfType<CollisionDetection>();
            yield return null;
        }
        Debug.Log("collision detection got!");
        //collisionDetection.OnSelectCorrectAnswer += IncreaseScore;
        //collisionDetection.OnSelectIncorrectAnswer += DecreaseScore;
        while (collisionDetection != null) 
        {
            yield return null;
        }
        StartCoroutine(TryGetCollisionDetection());
    }
    void Start()
    {
        StartCoroutine(TryGetCollisionDetection());
        answerSpawner = FindObjectOfType<AnswerSpawner>();
        answerSpawner.OnStartNextQuestion += SetQuestionText;
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }
    private void OnDestroy()
    {
        Debug.Log("On Destroy");
        if (collisionDetection != null) 
        {
            //collisionDetection.OnSelectCorrectAnswer -= IncreaseScore;
            //collisionDetection.OnSelectIncorrectAnswer -= DecreaseScore;
        }

    }
    private void OnEnable()
    {
        if (collisionDetection != null)
        {
            //collisionDetection.OnSelectCorrectAnswer += IncreaseScore;
            //collisionDetection.OnSelectIncorrectAnswer += DecreaseScore;
        }
    }

    public void IncreaseScore() 
    {
        Debug.Log("increasing score");
        score++;
        SetScoreText(score);
    }
    public void DecreaseScore() 
    {
        Debug.Log("decrease score");
        score--;
        SetScoreText(score);
    }

}
