using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DragonController : MonoBehaviour
{
    [SerializeField] AnimationCurve fallCurve;
    Animator animator;
    bool selected;
    Camera cam;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play("LD Fly Idle", Random.Range(0, 1));
        cam = Camera.main;
    }
    public void SetSelected(bool correct)
    {
        string trigger = correct ? "Correct" : "Incorrect";
        animator.SetTrigger(trigger);
        selected = true;
    }

    public void StartFlyUpOffScreen() 
    {
        StartCoroutine(FlyUpOffScreen());
    }
    IEnumerator FlyUpOffScreen() 
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        while (viewportPos.y < 2) 
        {
            transform.position += transform.up * Time.deltaTime;
            viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            yield return null;
        }
        Debug.Log("Destroyed!");
        Destroy(this.gameObject);
    }
    public void StartFallOffScreen() 
    {
        StartCoroutine(FallOffScreen());
    }
    IEnumerator FallOffScreen() 
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        while(viewportPos.y > -1) 
        {
            transform.position -= (transform.up) * Time.deltaTime;
            viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            yield return null;
        }
        Debug.Log("Destroyed!");
        Destroy(this.gameObject);
    }
    private void Update()
    {
        if (selected) return;
        transform.forward = (cam.transform.position - transform.position).normalized;
    }


}
