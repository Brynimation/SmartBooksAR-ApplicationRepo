using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


//TO DO: This class is very similar to the CollisionDetection. Maybe they should both inherit from a more generic parent class.
public class WallSelection : MonoBehaviour
{
    public Action<float> OnUpdateTimeRemaining; //Event - invoked when the ui needs to display the current time remaining

    [SerializeField] LayerMask answerLayerMask;
    [SerializeField] Color originalColour;
    [SerializeField] Color selectedColour;
    public MeshRenderer mRenderer;
    public float selectionTime;

    float remainingSelectionTime;
    bool selecting = false;
    bool selected = false;
    WallGenerator wallGenerator;

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        wallGenerator = FindObjectOfType<WallGenerator>();
        wallGenerator.OnSpawnNewValues += (int x, int y) => { selected = false; };
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
        if (selected) return;
        Vector3 centreScreenSpace = Camera.main.WorldToScreenPoint(mRenderer.bounds.center);
        Ray ray = Camera.main.ScreenPointToRay(centreScreenSpace);
        RaycastHit hit;

        //As with the CollisionDetection class, we use a spherecast to project a sphere along a ray, returning true if the sphere collides with anything.
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
}
