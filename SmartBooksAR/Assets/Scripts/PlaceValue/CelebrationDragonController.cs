using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelebrationDragonController : MonoBehaviour
{
     Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Celebrating", true);
        Material mat = GetComponentInChildren<Renderer>().material;
        mat.color = GenerateRandomColour();
        transform.forward = (Camera.main.transform.position - transform.position).normalized;
    }
    public void SetSelected() 
    {
        animator.SetTrigger("Correct");
        StartCoroutine(FlyUpOffScreen());
    }

    public void Jump() 
    {
        animator.SetTrigger("Jump");
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
    private Color GenerateRandomColour() 
    {
        return new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));
    }
    void Update()
    {
        
    }
}
