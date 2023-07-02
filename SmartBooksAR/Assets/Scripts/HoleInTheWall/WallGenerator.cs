using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
struct WallGeneratorValues 
{
    public int[] values;
}
public class WallGenerator : MonoBehaviour
{
    public Action<int, int, bool> OnSpawnNewValues;
    public Action<string> OnDisplayMessage;
    public Action<string> OnDisplayLargeText;

    [SerializeField] Vector2 minMaxViewportXPos;
    [SerializeField] List<int> targetValues;
    [SerializeField] List<int> currentValues;
    [SerializeField] WallPiece wallPiecePrefab;
    [SerializeField] List<WallGeneratorValues> wallGeneratorValues;
    [SerializeField] float wallYViewportPos;
    [SerializeField] float distanceFromNearClipPlane = 5f;
    [SerializeField] WallPiece[] wallPieces;
    [SerializeField] float screenTime = 5f;
    [SerializeField] Color incrementColour;
    [SerializeField] Color decrementColour;

    ARFace ARFace;
    Renderer faceRenderer;
    int currentValueIndex;
    WallCollisionDetection wallSelection;
    bool coroutineFinished;
    bool isSecondAttempt;

    Camera cam;
    float elapsedTime;
    private float GetScreenSpaceDiagonal(Bounds bounds)
    {
        Vector3 min = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 max = Camera.main.WorldToScreenPoint(bounds.max);
        return (max - min).magnitude;
    }

    private void InitialiseAnswers() 
    {
        for (int i = 0; i < 4; i++)
        {
            Color colour = (wallGeneratorValues[currentValueIndex].values[i] > 0) ? incrementColour : decrementColour;
            bool isCorrect = (currentValues[currentValueIndex] + wallGeneratorValues[currentValueIndex].values[i]) == targetValues[currentValueIndex];
            wallPieces[i].InitialiseAnswer(0, wallGeneratorValues[currentValueIndex].values[i], distanceFromNearClipPlane, colour, isCorrect);
        }
    }

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        WallPiece.OnPieceSelected += CheckIfCorrect;
    }


    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(DelayStart());
    }

    void CheckIfCorrect(int val, bool isCorrect) 
    {
        if (!isSecondAttempt && !isCorrect)
        {
            OnDisplayMessage("Try again");
            currentValueIndex--;
            isSecondAttempt = true;
        }
        else {
            OnDisplayMessage("");
            isSecondAttempt = false;
        }
    }
    IEnumerator DelayStart()
    {
        //TO DO: This code is used in the BalloonSpawner class too. Could it be moved to a separate class?
#if !UNITY_EDITOR
        while (ARFace == null) 
        {
            ARFace = FindObjectOfType<ARFace>();
            yield return null;
        }
        faceRenderer = ARFace.GetComponent<Renderer>();
        //Tell the player to stand further back
        OnDisplayMessage?.Invoke("Move away from the camera");

        //Only start the game once the player is far enough away from the camera
        float faceScreenSize = GetScreenSpaceDiagonal(faceRenderer.bounds);
        while (ARFace == null || faceScreenSize <= 0 || GetScreenSpaceDiagonal(faceRenderer.bounds) > 500) 
        {
            if(ARFace == null)
            {
                ARFace = FindObjectOfType<ARFace>();
                if(ARFace != null){
                    faceRenderer = ARFace.GetComponent<Renderer>();
                    yield return null;
                }
            }
            Debug.Log("faceRenderer: "+faceRenderer);
            yield return null;
        }
#endif
        while (wallSelection == null) 
        {
            wallSelection = FindObjectOfType<WallCollisionDetection>();
            yield return null;
        }
        OnDisplayMessage?.Invoke("");
        OnDisplayLargeText?.Invoke(3.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(2.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(1.ToString());
        yield return new WaitForSeconds(1f);
        elapsedTime = screenTime;
        //SpawnNextQuestion();
        coroutineFinished = true;
        //InitialiseAnswers();

    }
    /*void Start2()
    {
        wallPieces = new WallPiece[4];
        for (int i = 0; i < 4; i++) 
        {
            float viewportPosY = Mathf.Lerp(minMaxViewportY.x, minMaxViewportY.y, (float)i / 4f);
            wallPieces[i] = Instantiate(wallPiecePrefab, Vector3.zero, Quaternion.identity);
            wallPieces[i].InitialiseAnswer(viewportPosY, i, true, distanceFromNearClipPlane);
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        if (wallSelection != null && coroutineFinished)
        {
            float viewportPosX = Mathf.Lerp(minMaxViewportXPos.y, minMaxViewportXPos.x, elapsedTime / screenTime);
            transform.position = cam.ViewportToWorldPoint(new Vector3(viewportPosX, wallYViewportPos, cam.nearClipPlane + distanceFromNearClipPlane));
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= screenTime)
            {
                SpawnNextQuestion();
            }
        }
        else {
            wallSelection = FindObjectOfType<WallCollisionDetection>();
        }

    }

    private void SpawnNextQuestion() 
    {
        if (currentValueIndex == targetValues.Count)
        {
            OnSpawnNewValues?.Invoke(0, 0, false);
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("here!");
            OnSpawnNewValues?.Invoke(currentValues[currentValueIndex], targetValues[currentValueIndex], isSecondAttempt);
            InitialiseAnswers();
            elapsedTime = 0f;
            currentValueIndex++;
        }
    }
}
