using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishSystemController : MonoBehaviour
{
    public ParticleSystem vanishSystem;
    public Camera camera;
    public GameObject targetObject;
    public GameObject targetSwitch;
    public GameObject nextTarget;
    public float setZ;
    public float timeInterval;
    public float fadeTime;
    public int maxParticles;

    private Renderer renderer;
    private float time;
    private ParticleSystem.Particle[] particles;
    private float alphaSet;
    private bool fadeIn;
    private bool fadeOut;

    void OnEnable()
    {
        Debug.Log("emit vanish");
        vanishSystem.Emit(1);
        time = 0f;
        alphaSet = 0f;
        fadeIn = false;
        fadeOut = false;
        targetObject = null;
        targetSwitch = null;
    }

    void Update()
    {
        //Finds targets
        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Dancer");
        }
        if (targetSwitch == null)
        {
            targetSwitch = GameObject.FindGameObjectWithTag("Target");
        }
        //if video has played already, then indicator points towards the spiral (targetSwitch), otherwise, it points towards dancers
        nextTarget = GameObject.FindObjectOfType<IndicatorSystemController>().isDone ? targetSwitch : targetObject;
        renderer = nextTarget.transform.GetChild(0).GetComponent<Renderer>();
        time += Time.deltaTime;

        particles = new ParticleSystem.Particle[maxParticles];
        int num = vanishSystem.GetParticles(particles);

        var screenPos = GetScreenPos();
        moveParticle(screenPos); //particle should be on the edge of the screen
        particles[0] = rotateParticle(particles[0], screenPos); //particle must orient itself such that longer base is on the side of the screen

        var shouldShow = ShouldShow();
        if (shouldShow)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        var timerShouldShow = TimerShouldShow();
        var globalShow = shouldShow && timerShouldShow;
        /*
         * particle should only show if 
         * 1) the dancer is not within the user's field of view and
         * 2) enough time has since the particle last appeared
         */
        if (globalShow)
        {
            fadeOut = false;
            fadeIn = true;
        }
        else
        {
            fadeOut = true;
            fadeIn = false;
        }
        if (fadeIn)
        {
            alphaSet = alphaSet + 1f / fadeTime * Time.deltaTime;
            if (alphaSet > 1)
            {
                alphaSet = 1;
                fadeIn = false;
            }
        }
        if (fadeOut)
        {
            alphaSet = alphaSet - 1f / fadeTime * Time.deltaTime;
            if (alphaSet < 0)
            {
                alphaSet = 0;
                fadeOut = false;
            }
        }
        var color = particles[0].startColor;
        color.a = (byte) (alphaSet * 255);
        particles[0].startColor = color;

        vanishSystem.SetParticles(particles);
    }
    
    /*
     * finds the correct location along the side of the screen the particle should be at
     * the particle should be on the side of the screen that is closest to the position of the target
     */
    private Vector3 GetScreenPos()
    {
        Vector3 screenPos = camera.WorldToScreenPoint(nextTarget.transform.position);
        screenPos = ForceToSide(screenPos);
        return screenPos;
    }

    /*
     * transforms the position of the particle from an arbitrary location onto a side of the screen
     */
    private Vector3 ForceToSide(Vector3 screenPos)
    {
        int halfWidth = camera.pixelWidth / 2, halfHeight = camera.pixelHeight / 2;
        screenPos.x -= halfWidth;
        screenPos.y -= halfHeight;

        screenPos.x *= Mathf.Sign(screenPos.z);
        screenPos.y *= Mathf.Sign(screenPos.z);

        Vector3 byX = new Vector3(screenPos.x * Mathf.Abs(halfWidth / screenPos.x), screenPos.y * Mathf.Abs(halfWidth / screenPos.x), setZ);
        Vector3 byY = new Vector3(screenPos.x * Mathf.Abs(halfHeight / screenPos.y), screenPos.y * Mathf.Abs(halfHeight / screenPos.y), setZ);
        
        if(Mathf.Abs(byX.x) <= halfWidth + 5 && Mathf.Abs(byX.y) <= halfHeight + 5)
        {
            byX.x += halfWidth;
            byX.y += halfHeight;
            return byX;
        }
        byY.x += halfWidth;
        byY.y += halfHeight;
        return byY;
    }

    private void moveParticle(Vector3 screenPos)
    {
        vanishSystem.transform.position = camera.ScreenToWorldPoint(screenPos);
        //Debug.Log(screenPos);
    }
    /*
     * the vanish system uses a trapezoidal particle
     * rotates the particle so that it's correctly oriented such that the longer base is on the side of the screen
     */
    private ParticleSystem.Particle rotateParticle(ParticleSystem.Particle particle, Vector3 screenPos)
    {
        if (Within(screenPos.y, 0, 5))
        {
            particle.rotation = 180;
        }
        else if (Within(screenPos.y, camera.pixelHeight, 5))
        {
            particle.rotation = 0;
        }
        else if (Within(screenPos.x, 0, 5))
        {
            particle.rotation = 270;
        }
        else if (Within(screenPos.x, camera.pixelWidth, 5))
        {
            particle.rotation = 90;
        }
        Debug.Log(particle.rotation);
        return particle;
    }
    /*
     * the particle only shows up if the user is not looking in the direction of the target
     */
    private bool ShouldShow()
    {
        if ((nextTarget.tag == "Target" && nextTarget.GetComponent<Renderer>().isVisible) || 
            (nextTarget.tag == "Dancer" && nextTarget.transform.GetChild(0).GetComponent<Renderer>().isVisible))
        {
            return false;
        }
        return true;
    }

    /*
     * the particle only shows up for a few seconds, and then disappears, and then shows up again
     */
    private bool TimerShouldShow()
    {
        if (time > timeInterval)
        {
            StartCoroutine(waitTime());
            return true;
        }
        return false;
    }
    IEnumerator waitTime()
    {
        yield return new WaitForSeconds(timeInterval);
        time = 0;
    }

    private bool Within(float a, float b, float t)
    {
        return Mathf.Abs(a - b) <= t;
    }
}
