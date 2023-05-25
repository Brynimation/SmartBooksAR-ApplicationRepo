using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] UnityEvent OnAnswerSelected;
    [SerializeField] Vector2 viewportCoords;
    private void Update()
    {
        Vector3 viewportPosition = new Vector3(viewportCoords.x, viewportCoords.y, Camera.main.nearClipPlane + 1f);
        transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}
