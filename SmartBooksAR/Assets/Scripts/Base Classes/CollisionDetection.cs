using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionDetection : MonoBehaviour
{
    [SerializeField] protected LayerMask answerLayerMask;
    protected Renderer mRenderer;

    protected virtual void Awake() 
    {
        mRenderer = GetComponent<Renderer>();
    }
    protected abstract void Select();

    protected virtual Ray CastRayThroughBoundsCentre() 
    {
        Vector3 centreScreenSpace = Camera.main.WorldToScreenPoint(mRenderer.bounds.center);
        return Camera.main.ScreenPointToRay(centreScreenSpace);
    }
}
