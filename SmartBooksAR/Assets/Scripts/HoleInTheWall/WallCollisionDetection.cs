using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


//TO DO: This class is very similar to the BalloonCollisionDetection. Maybe they should both inherit from a more generic parent class.
public class WallCollisionDetection : CollisionDetection
{
    public Action<float> OnUpdateTimeRemaining; //Event - invoked when the ui needs to display the current time remaining

    [SerializeField] Color selectedColour;
    public float selectionTime;

    float remainingSelectionTime;
    bool selecting = false;
    bool selected = false;
    WallGenerator wallGenerator;

    protected override void Awake()
    {
        base.Awake();
        wallGenerator = FindObjectOfType<WallGenerator>();
        wallGenerator.OnSpawnNewValues += (int x, int y) => { selected = false; };
    }
    protected override void Select()
    {
        if (selected) return;
        Ray ray = CastRayThroughBoundsCentre();
        RaycastHit hit;

        //As with the BalloonCollisionDetection class, we use a spherecast to project a sphere along a ray, returning true if the sphere collides with anything.
        //This first if-statement is true when the user has just started selecting a wallPiece using their face
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude / 2f, out hit, Mathf.Infinity, answerLayerMask) && !selecting)
        {
            selecting = true;
        }
        //This second if-statement is true when the user is continuing to select a wallPiece.
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude / 2f, out hit, Mathf.Infinity, answerLayerMask) && selecting)
        {
            remainingSelectionTime -= Time.deltaTime;
            OnUpdateTimeRemaining?.Invoke((float)Math.Round(remainingSelectionTime, 2));
            //This nested if-statement is called if the user has held their face on a wallPiece long enough for it to be selected 
            if (remainingSelectionTime < 0)
            {
                remainingSelectionTime = selectionTime;
                WallPiece wallPiece = hit.collider.GetComponent<WallPiece>();
                if (wallPiece != null)
                {
                    wallPiece.SetSelected(selectedColour);
                    selecting = false;
                    selected = true;
                }

            }
        }
    }
    private void Start()
    {
        remainingSelectionTime = selectionTime;
    }
    private void OnDestroy()
    {
        OnUpdateTimeRemaining?.Invoke(0f);
        remainingSelectionTime = selectionTime;
    }
    private void OnDisable()
    {
        OnUpdateTimeRemaining?.Invoke(0f);
        remainingSelectionTime = selectionTime;
    }
    void Update()
    {
        Select();

    }
}
