using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.XR.ARFoundation;

public class Balloon : MonoBehaviour
{
    public static Action<int> OnDestroyBalloon;
    public Vector2 viewportPosition;
    public Canvas worldCanvas;
    public TMP_Text answerText;
    public float distanceFromNearClipPlane;
    public Renderer mRenderer;
    public Renderer faceRenderer;
    public GameObject correctParticleExplosion;
    public GameObject incorrectParticleExplosion;
    public float xAmplitude;
    public float yAmplitude;
    public float balloonMaxSpeed = 1f;

    private Material material;
    private float currentXVelocity;
    private bool selected;
    private float currentYVelocity;
    private bool collided = false;
    private static int totalBalloons = 0;
    private int balloonId;
    private ARFace ARFace;
    private GameObject explosionToInstantiate;
    private Collider col;
    private Rigidbody rb;


    private void OnEnable()
    {
        balloonId = totalBalloons;
        totalBalloons++;
    }
    private void Awake()
    {

        //Due to the differences between how the balloons behave in editor and how they behave when built to the phone, the speed of the balloons is increased by a factor of 10 in builds.
#if !UNITY_EDITOR
    xAmplitude *= 10;
    yAmplitude *= 10;
#endif

        mRenderer = GetComponent<Renderer>();
        material = mRenderer.material;
#if UNITY_EDITOR
        faceRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
#endif
        worldCanvas = GetComponentInChildren<Canvas>();
        col = GetComponent<Collider>();

        //We multiply localscale.x of the text on the balloons by -1 in builds. This is due to the fact that in builds the front facing camera is used, so text in the world appears flipped.
#if !UNITY_EDITOR
        Vector3 localScale = answerText.transform.localScale;
        localScale.x *= -1;
        answerText.transform.localScale = localScale;
        StartCoroutine(TryGetFace());
#endif
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    //Coroutine that searches for the a face. This runs in the background (ie, not on Unity's main thread) and so does not freeze up the rest of the application.
    IEnumerator TryGetFace()
    {
        ARFace = FindObjectOfType<ARFace>();
        while (ARFace == null)
        {
            ARFace = FindObjectOfType<ARFace>();
            yield return null;
        }
        faceRenderer = ARFace.gameObject.GetComponent<Renderer>();
    }
    //Sets the position and text of this balloon
    public void InitialiseAnswer(Vector2 viewportPos, string answer, int curIndex, List<int> correctIndices, Renderer faceRenderer)
    {
        viewportPosition = viewportPos;
        answerText.SetText(answer);
        gameObject.tag = (correctIndices.Contains(curIndex)) ? "Correct" : "Incorrect";
        Vector3 viewport = new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane + distanceFromNearClipPlane);
        Vector3 pos = Camera.main.ViewportToWorldPoint(viewport);

        //This if statement ensures balloons don't spawn on the face
        if (faceRenderer != null && Vector3.Distance(faceRenderer.bounds.center, pos) <= faceRenderer.bounds.extents.magnitude * 2)
        {
            Vector3 dir = pos - faceRenderer.bounds.center;
            pos += dir * 3;
        }
        transform.position = pos;
    }

    //When balloons collide, a force is exerted on this balloon so they bounce off of eachother.
    private void OnCollisionEnter(Collision collision)
    {
        Balloon balloon = collision.gameObject.GetComponent<Balloon>();
        if (balloon != null)
        {
            collided = true;
            Vector3 forceDir = rb.position - balloon.rb.position;
            rb.velocity = forceDir * 10f;
        }
    }
    public void IncorrectAnswerFade(Color incorrectColour, float fadeTime) 
    {
        if (selected) return;
        selected = true;
        balloonMaxSpeed = 0f;
        StartCoroutine(IncorrectAnswerFadeCoroutine(incorrectColour, fadeTime));
    }
    private IEnumerator IncorrectAnswerFadeCoroutine(Color incorrectColour, float fadeTime) 
    {
        Color originalColour = material.color;
        float timeElapsed = 0f;
        while (timeElapsed < fadeTime) 
        {
            material.color = Color.Lerp(originalColour, incorrectColour, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    } 

    //These calculations are performed in FixedUpdate as they are physics calculations; we're manipulating a rigidbody.
    private void FixedUpdate()
    {
        //Before a balloon has collided with another, it follows a periodic, circular path.
        if (!collided)
        {
            rb.velocity += new Vector3(xAmplitude * Mathf.Sin((float)balloonId + Time.time / 5f), yAmplitude * Mathf.Cos((float)balloonId + Time.time / 5f), 0f) * Time.fixedDeltaTime;
        }
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, balloonMaxSpeed);

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(rb.position);
        //If a balloon goes too far off screen horizontally in either direction, a force is exerted on the balloon which dictates its new velocity.
        if (screenPoint.x <= 0 || screenPoint.x >= Screen.width)
        {
            collided = true;
            float dir = (screenPoint.x <= 0) ? 1f : -1f;
#if !UNITY_EDITOR //accounting for values being flipped with the front facing camera
                dir *= -1;
#endif
            Vector3 vel = rb.velocity;
            vel.x = Mathf.SmoothDamp(vel.x, dir * 10f, ref currentXVelocity, 1f);
            rb.velocity = vel;
        }
        //If a balloon goes too far off screen vertically in either direction, a force is exerted on the balloon which dictates its new velocity.
        if (screenPoint.y <= 0 || screenPoint.y >= Screen.height)
        {
            collided = true;
            float dir = (screenPoint.y <= 0) ? 1f : -1f;
            Vector3 vel = rb.velocity;
            vel.y = Mathf.SmoothDamp(vel.y, dir * 10f, ref currentXVelocity, 1f);
            rb.velocity = vel;
        }

    }
    private void OnDrawGizmos()
    {
        if (mRenderer == null || faceRenderer == null) return;
        Gizmos.DrawWireCube(mRenderer.bounds.center, mRenderer.bounds.extents * 2);
        Gizmos.DrawWireCube(faceRenderer.bounds.center, faceRenderer.bounds.extents * 2);
    }
}
 
