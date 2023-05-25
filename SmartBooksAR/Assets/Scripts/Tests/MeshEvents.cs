using System;
using UnityEngine;

public class MeshEvents : MonoBehaviour
{
    MeshRenderer faceRenderer;

    public event Action OnSelectCorrectAnswer;
    public event Action OnSelectIncorrectAnswer;

    Bounds correctAnswerMeshBounds;
    Bounds incorrectAnswerMeshBounds;
    Bounds meshColliderBounds;
    MeshRenderer player;

    struct ScreenSpaceBounds
    {
        public Vector2 centre;
        public Vector2 max;
        public Vector2 min;

        public ScreenSpaceBounds(Vector2 centre, Vector2 max, Vector2 min)
        {
            this.centre = centre;
            this.max = max;
            this.min = min;
        }
    }
    void Start()
    {
        faceRenderer = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<MeshRenderer>();
        meshColliderBounds = player.bounds;
        correctAnswerMeshBounds = GameObject.FindGameObjectWithTag("Correct").GetComponent<MeshRenderer>().bounds;
        incorrectAnswerMeshBounds = GameObject.FindGameObjectWithTag("Incorrect").GetComponent<MeshRenderer>().bounds;
    }

    // Update is called once per frame
    private Vector3 SetZCoord(Vector3 originalCoord, float newZ) 
    {
        originalCoord.z = newZ;
        return originalCoord;
    }
    void Update()
    {
        //creating a screen space, bounding rectangle around the meshes

        Vector2 centreScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(player.bounds.center, Camera.main.nearClipPlane + 1f));
        Vector2 maxPointScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(player.bounds.max, Camera.main.nearClipPlane + 1f));
        Vector2 minPointScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(player.bounds.min, Camera.main.nearClipPlane + 1f));

        ScreenSpaceBounds faceScreenSpaceBounds = new ScreenSpaceBounds(centreScreenSpace, maxPointScreenSpace, minPointScreenSpace);

        Vector2 incorrectCentreScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(incorrectAnswerMeshBounds.center, Camera.main.nearClipPlane + 1f));
        Vector2 incorrectMaxPointScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(incorrectAnswerMeshBounds.max, Camera.main.nearClipPlane + 1f));
        Vector2 incorrectMinPointScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(incorrectAnswerMeshBounds.min, Camera.main.nearClipPlane + 1f));

        ScreenSpaceBounds incorrectMeshScreenSpaceBounds = new ScreenSpaceBounds(incorrectCentreScreenSpace, incorrectMinPointScreenSpace, incorrectMaxPointScreenSpace);

        Vector2 correctCentreScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(correctAnswerMeshBounds.center, Camera.main.nearClipPlane + 1f));
        Vector2 correctMaxPointScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(correctAnswerMeshBounds.max, Camera.main.nearClipPlane + 1f));
        Vector2 correctMinPointScreenSpace = Camera.main.WorldToScreenPoint(SetZCoord(correctAnswerMeshBounds.min, Camera.main.nearClipPlane + 1f));

        ScreenSpaceBounds correctMeshScreenSpaceBounds = new ScreenSpaceBounds(correctCentreScreenSpace, correctMinPointScreenSpace, correctMaxPointScreenSpace);
        Debug.Log("centre: " + faceScreenSpaceBounds.centre + "incorrect centre: " + incorrectMeshScreenSpaceBounds.centre);

        if (DetectScreenSpaceCollision(faceScreenSpaceBounds, correctMeshScreenSpaceBounds)) 
        {
            OnSelectCorrectAnswer?.Invoke();
            Debug.Log("correct collision");
        }
        if (DetectScreenSpaceCollision(faceScreenSpaceBounds, incorrectMeshScreenSpaceBounds)) 
        {
            OnSelectIncorrectAnswer?.Invoke();
            Debug.Log("inorrect collision");
        }
    }

    bool DetectScreenSpaceCollision(ScreenSpaceBounds col1, ScreenSpaceBounds col2) 
    {
        //if col1.min.x > col2.max.x OR col1.min.y > col2.max.y return false
        //if col2.min.x > col1.max.x OR col2.min.y > col1.max.y return false
        //else return true
        //return true;
        return !((col1.min.x > col2.max.x || col1.min.y > col2.max.y)
                || (col2.min.x > col1.max.x || col2.min.y > col1.max.y)
            );
    }
}
