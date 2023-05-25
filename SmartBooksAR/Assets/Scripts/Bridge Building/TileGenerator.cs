using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


[System.Serializable]
public struct NumberRepresentation
{
    public Sprite targetCorrectRepresentation;

    public List<Sprite> equivalentRepresentations;
    public List<int> correctAnswerIndices;

}
public class TileGenerator : MonoBehaviour
{
    //Events
    public Action<Sprite> OnSpawnNextQuestion; 
    public Action<string> OnDisplayLargeText;
    public Action<string> OnDisplayMessage;
    public Action OnAllCorrectSelected;
 
    /*These two minMax variables define the region of the camera's view volume that the tiles may appear in. The x component of the vector2 stores the minimum value, and the y component the maximum. 
     * Viewport space in unity is defined from (0,0) at the bottom left of the screen to (1,1) at the top right.*/
    public Vector2 minMaxXViewportPos = new Vector2(0.3f, 0.9f);
    public Vector2 minMaxYViewportPos = new Vector2(0.2f, 0.8f);
    public GameObject tilePrefab;
    public List<NumberRepresentation> numberRepresentations;

    private ARFace ARFace;
    Renderer faceRenderer;
    bool coroutineComplete = false;
    private int currentQuestionIndex;
    private bool started;
    int numCorrectSelected;
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    //TO DO: This same function is used in the BalloonSpawner class. It should be moved to its own class.
    private float GetScreenSpaceDiagonal(Bounds bounds)
    {
        Vector3 min = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 max = Camera.main.WorldToScreenPoint(bounds.max);
        return (max - min).magnitude;
    }
    void OnTileSelected(bool selected)
    {
        numCorrectSelected += (selected) ? 1 : 0;
        //If the user has selected all correct tiles then destroy the current tiles and spawn the appropriate tiles for the next question
        //So that the tiles always form a "path" from one side of the screen to the other, there must always be 4 answers.
        if (numCorrectSelected == 4)
        {
            OnAllCorrectSelected?.Invoke();
            numCorrectSelected = 0;
            Tile[] tiles = FindObjectsOfType<Tile>();
            foreach (Tile t in tiles) 
            {
                Destroy(t.gameObject);
            }
            if (currentQuestionIndex == numberRepresentations.Count)
            {
                OnDisplayLargeText?.Invoke("Complete!");
                return;
            }
            StartCoroutine(SpawnNextTiles());
        }
    }
    IEnumerator SpawnNextTiles() 
    {
        OnDisplayMessage?.Invoke("Next question in...");
        OnDisplayLargeText?.Invoke(3.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(2.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(1.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayMessage("");
        SpawnAllTiles();
    }
    void Start()
    {
        StartCoroutine(DelayStart());
        Tile.OnSelected += OnTileSelected;

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
            yield return null;
        }
#endif
        OnDisplayMessage?.Invoke("");
        OnDisplayLargeText?.Invoke(3.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(2.ToString());
        yield return new WaitForSeconds(1f);
        OnDisplayLargeText?.Invoke(1.ToString());
        yield return new WaitForSeconds(1f);

        SpawnAllTiles();

    }

    public void SpawnAllTiles() 
    {

        if(currentQuestionIndex == 0) 
        {
            OnSpawnNextQuestion?.Invoke(numberRepresentations[currentQuestionIndex].targetCorrectRepresentation);
        }
        List<Sprite> representations = numberRepresentations[currentQuestionIndex].equivalentRepresentations;
        List<int> correctIndices = numberRepresentations[currentQuestionIndex].correctAnswerIndices;

        int index = 0;
        for (int x = 0; x < 4; x++) //4 columns
        {
            for (int y = 0; y < 3; y++) //3 rows
            {
                Sprite representation = representations[index];
                float xPos = Mathf.Lerp(minMaxXViewportPos.x, minMaxXViewportPos.y, (float)x / 3f); //evenly space the tiles between the minimum and maximum viewport coordinates
                float yPos = Mathf.Lerp(minMaxYViewportPos.x, minMaxYViewportPos.y, (float)y / 2f);
                Vector2 viewportPos = new Vector2(xPos, yPos);
                SpawnTile(viewportPos, representation, index, correctIndices);
                index++;
            }
        }
        //Send event to UI to change the question
        OnSpawnNextQuestion?.Invoke(numberRepresentations[currentQuestionIndex].targetCorrectRepresentation);

        //Move to next quesetion
        currentQuestionIndex++;
    }

    //Instantiate a tile with the appropriate image at the appropriate position
    private void SpawnTile(Vector2 viewportPosition, Sprite answerSprite, int curIndex, List<int> correctIndices) 
    {
        Tile tile = Instantiate(tilePrefab, transform.position, Quaternion.identity).GetComponent<Tile>();
        tile.InitialiseAnswer(viewportPosition, answerSprite, curIndex, correctIndices);
    }


    void Update()
    {
        //TO DO: This code is used in the BalloonSpawner class too. Could it be moved to a separate class?
#if !UNITY_EDITOR
        if (ARFace == null && started) 
        {
            started = false;
            ARFace = FindObjectOfType<ARFace>();
            if(ARFace != null) 
            {
                faceRenderer = ARFace.GetComponent<Renderer>();
                started = true;
            }
        }

        if (ARFace == null && coroutineComplete)
        {
            ARFace = FindObjectOfType<ARFace>();
            if(ARFace != null) 
            {
                faceRenderer = ARFace.GetComponent<Renderer>();
                started = true;
            }
        }
#endif
        if (!started) return;

    }
}
