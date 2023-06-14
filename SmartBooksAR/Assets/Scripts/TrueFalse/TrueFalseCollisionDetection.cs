using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/*TO DO:
 FLIP TEXT DIRECTION OF TRUE FALSE REGIONS
ADJUST SIZE AND SHAPE OF REGIONS
MAKE THE MOVE AWAY FROM CAMERA TEXT APPEAR
 */

public class TrueFalseCollisionDetection : CollisionDetection
{
    public Action<bool> OnSelectAnswer;
    public float selectionTime;
    [SerializeField] private float timeElapsed = 0f;
    static bool started;
    private bool answerSelected;
    TrueFalseQuestionSpawner tfQuestionSpawner;
    protected override void Awake()
    {
        base.Awake();
        tfQuestionSpawner = FindObjectOfType<TrueFalseQuestionSpawner>();
        tfQuestionSpawner.OnSpawnNextQuestion += NextQuestion;
        tfQuestionSpawner.OnGameStarted += OnGameStarted;
    }

    protected override void Select()
    {
        if (!started) return;
        
        Ray ray = CastRayThroughBoundsCentre();
        RaycastHit hit;
        //A spherecast acts like a "thick" raycast - we project a sphere with a radius equal to the radius of the target on the user's face along the ray.
        //If any answers collide with this projected sphere, then the answer is destroyed.
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude / 2f, out hit, answerLayerMask) && timeElapsed > selectionTime)
        {
            Debug.Log("sphere casting");
            timeElapsed = 0f;
            string tag = hit.collider.tag;
            TrueFalseRegion tfRegion = hit.collider.gameObject.GetComponent<TrueFalseRegion>();
            tfRegion.SetSelected(tfRegion.IsTrue == tfQuestionSpawner.currentCorrectAnswer);
            OnSelectAnswer?.Invoke(tfRegion.IsTrue);
            answerSelected = true;
        }
        timeElapsed += Time.deltaTime;
    }
    private void NextQuestion(string question) {
        answerSelected = false;
        timeElapsed = 0f;
        
    }
    private void OnGameStarted() 
    {
        Debug.Log("Started");
        started = true;
    }


    // Update is called once per frame
    void Update()
    {
        Select();
    }
}
