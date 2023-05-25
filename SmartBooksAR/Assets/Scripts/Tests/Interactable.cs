using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] Color normalColour;
    [SerializeField] Color selectedColour;
    Collider col;
    Renderer rend;
    Material mat;
    bool isActive;
    [SerializeField] LayerMask interactableMask;
    private void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        mat = rend.material;
    }

    public bool IsActive
    {
        get { return isActive;}
        set { isActive = value; }
    }

    public void SetActive(bool state) 
    {
        Color colour = (state) ? selectedColour : normalColour;
        mat.color = colour;
    }
    public void Rotate(Vector2 delta) 
    {
        //transform.Rotate(delta.y, delta.x, 0f);
        transform.Rotate(Vector3.right, delta.y, Space.World);
        transform.Rotate(Vector3.up, delta.x, Space.World);
    }

}
