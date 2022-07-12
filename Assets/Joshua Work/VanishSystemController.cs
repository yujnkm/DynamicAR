using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishSystemController : MonoBehaviour
{
    public ParticleSystem vanishSystem;
    public Camera camera;
    public GameObject targetObject;
    public GameObject targetSwitch;
    public float setZ;
    public float timeInterval;
    public int maxParticles;

    private Renderer renderer;
    private float time;
    private ParticleSystem.Particle[] particles;
    private GameObject nextTarget;
    private float alphaSet;

    void Start()
    {
        targetObject = GameObject.FindGameObjectWithTag("Dancer").transform.GetChild(0).gameObject;
        targetSwitch = GameObject.FindGameObjectWithTag("Target");
        vanishSystem.Emit(1);
        time = 0f;
        alphaSet = 0f;
    }

    void Update()
    {
        nextTarget = IndicatorSystemController.isDone ? targetSwitch : targetObject;
        renderer = nextTarget.GetComponent<Renderer>();
        time += Time.deltaTime;

        particles = new ParticleSystem.Particle[maxParticles];
        int num = vanishSystem.GetParticles(particles);

        var screenPos = GetScreenPos();
        moveParticle(screenPos);
        particles[0] = rotateParticle(particles[0], screenPos);

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
        if (globalShow)
        {
            alphaSet = Mathf.Clamp(alphaSet + Time.deltaTime, 0, 1);
        }
        else
        {
            alphaSet = Mathf.Clamp(alphaSet - Time.deltaTime, 0, 1);
        }
        var color = particles[0].startColor;
        color.a = (byte) (alphaSet * 255);
        particles[0].startColor = color;

        vanishSystem.SetParticles(particles, 1);
    }

    private Vector3 GetScreenPos()
    {
        Vector3 screenPos = camera.WorldToScreenPoint(nextTarget.transform.position);
        screenPos = ForceToSide(screenPos);
        return screenPos;
    }

    private Vector3 ForceToSide(Vector3 screenPos)
    {
        int halfWidth = camera.pixelWidth / 2, halfHeight = camera.pixelHeight / 2;
        screenPos.x -= halfWidth;
        screenPos.y -= halfHeight;

        screenPos.x *= Mathf.Sign(screenPos.z);
        screenPos.y *= Mathf.Sign(screenPos.z);

        Vector3 byX = new Vector3(screenPos.x * Mathf.Abs(halfWidth / screenPos.x), screenPos.y * Mathf.Abs(halfWidth / screenPos.x), setZ);
        Vector3 byY = new Vector3(screenPos.x * Mathf.Abs(halfHeight / screenPos.y), screenPos.y * Mathf.Abs(halfHeight / screenPos.y), setZ);
        
        if(Mathf.Abs(byX.x) <= halfWidth && Mathf.Abs(byX.y) <= halfHeight)
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
    }
    private ParticleSystem.Particle rotateParticle(ParticleSystem.Particle particle, Vector3 screenPos)
    {
        if (Within(screenPos.y, 0, 5))
        {
            particle.rotation = 180;
        }
        if (Within(screenPos.y, camera.pixelHeight, 5))
        {
            particle.rotation = 0;
        }
        if (Within(screenPos.x, 0, 5))
        {
            particle.rotation = 270;
        }
        if (Within(screenPos.x, camera.pixelWidth, 5))
        {
            particle.rotation = 90;
        }
        return particle;
    }

    private bool ShouldShow()
    {
        if(nextTarget.GetComponent<Renderer>().isVisible)
        {
            return false;
        }
        return true;
    }

    private bool TimerShouldShow()
    {
        int quo = (int)(time / timeInterval);
        if (quo % 2 == 0) return true;
        return false;
    }

    private bool Within(float a, float b, float t)
    {
        return Mathf.Abs(a - b) <= t;
    }
}
