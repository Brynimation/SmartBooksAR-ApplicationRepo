using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class MovingAnswer : MonoBehaviour
{
    [Header("Visuals")]
    public TMP_Text answerText;
    public float platformLength;
    public float platformHeight = 0.2f;
    public Canvas canvas;
    public bool isCorrectAnswer;
    public static Color correctColour = Color.green;
    public static Action OnSpawnNextQuestion;
    public static Color incorrectColour = Color.red;
    public float distanceFromNearClipPlane = 3f;
    public static int numAnswersLeftScreen = 0;
    [Range(0, 1)]
    public float viewportX;
    public static List<MovingAnswer> currentAnswers;

    [SerializeField] float totalTimeOnScreen;
    [SerializeField] Renderer rend;


    float elapsedTimeOnScreen;
    private void Awake()
    {
        if (currentAnswers == null) 
        {
            currentAnswers = new List<MovingAnswer>();
        }
    }
    void Start()
    {
        transform.localScale = new Vector3(platformLength, platformHeight, 0.01f);
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        answerText = GetComponentInChildren<TMP_Text>();
        Vector3 textLocalScale = new Vector3(1f / platformLength, 1f / platformHeight, 1f);
#if !UNITY_EDITOR
            textLocalScale.x *= -1;
#endif
        answerText.GetComponent<RectTransform>().localScale = textLocalScale;

    }
    private void OnEnable()
    {
        currentAnswers.Add(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
    public void SetAnswerProperties(Vector2 minMaxTimeOnScreen, Vector2 minMaxPlatformLength, bool isCorrectAnswer, float viewportX, string answer, int numAnswersToQuestion)
    {  
        this.isCorrectAnswer = isCorrectAnswer;
        this.platformLength = UnityEngine.Random.Range(minMaxPlatformLength.x, minMaxPlatformLength.y);
        this.totalTimeOnScreen = UnityEngine.Random.Range(minMaxTimeOnScreen.x, minMaxTimeOnScreen.y);
        this.viewportX = viewportX;
        rend.material.color = isCorrectAnswer ? correctColour : incorrectColour;
        gameObject.tag = isCorrectAnswer ? "Correct" : "Incorrect";
        answerText.SetText(answer);

    }

    // Update is called once per frame
    void Update()
    {
        float viewPortY = 1f - elapsedTimeOnScreen/totalTimeOnScreen;
        Vector3 viewportPosition = new Vector3(viewportX, viewPortY, Camera.main.nearClipPlane + distanceFromNearClipPlane);
        transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
        elapsedTimeOnScreen += Time.deltaTime;

    }

    private void OnBecameInvisible()
    {
        currentAnswers.Remove(this);
        Debug.Log("currentAnswers: "+ currentAnswers.Count);
        if (currentAnswers.Count == 0) 
        {
            Debug.Log("Dying");
            OnSpawnNextQuestion?.Invoke();
            Destroy(this.gameObject);
        }
    }
}
