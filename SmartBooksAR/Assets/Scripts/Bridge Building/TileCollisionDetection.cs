using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


//TO DO: This class is very similar to the BalloonCollisionDetection. Maybe they should both inherit from a more generic parent class.
public class TileCollisionDetection : CollisionDetection
{
    public Action<float> OnUpdateTimeRemaining; //Event - invoked when the ui needs to display the current time remaining

    [SerializeField] Color originalColour;
    public float selectionTime;

    float remainingSelectionTime;
    bool selecting = false;
    
    protected override void Select()
    {
        Ray ray = CastRayThroughBoundsCentre();
        RaycastHit hit;

        //As with the BalloonCollisionDetection class, we use a spherecast to project a sphere along a ray, returning true if the sphere collides with anything.
        //This first if-statement is true when the user has just started selecting a tile using their face
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude / 2f, out hit, Mathf.Infinity, answerLayerMask) && !selecting)
        {
            selecting = true;
        }
        //This second if-statement is true when the user is continuing to select a tile.
        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude / 2f, out hit, Mathf.Infinity, answerLayerMask) && selecting)
        {
            remainingSelectionTime -= Time.deltaTime;
            OnUpdateTimeRemaining?.Invoke((float)Math.Round(remainingSelectionTime, 2));
            //This nested if-statement is called if the user has held their face on a tile long enough for it to be selected 
            if (remainingSelectionTime < 0)
            {
                remainingSelectionTime = selectionTime;
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    string tag = hit.collider.tag;
                    Color targetColour = tag == "Correct" ? Color.green : Color.red;
                    //tile.SetColour(targetColour);
                    tile.SetSelected(); //the tile will either glow or dissolve depending on if it is correct or not
                    selecting = false;
                }

            }
        }
        else
        {
            selecting = false;
            remainingSelectionTime = selectionTime;
            OnUpdateTimeRemaining?.Invoke(0f);
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
