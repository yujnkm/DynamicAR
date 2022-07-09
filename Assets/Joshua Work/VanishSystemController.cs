using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishSystemController : MonoBehaviour
{
    public ParticleSystem vanishSystem;
    public Camera camera;
    public GameObject target;
    public float setZ;
    public float timeInterval;
    public int maxParticles;
    public float alphaSet;

    private Renderer renderer;
    private float time;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        renderer = target.GetComponent<Renderer>();
        particles = new ParticleSystem.Particle[maxParticles];
        time = 0f;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (renderer.isVisible && vanishSystem.particleCount > 0)
        {
            int num = vanishSystem.GetParticles(particles);
            for (int i = 0; i < num; i++)
            {
                particles[i].remainingLifetime = Mathf.Min(alphaSet * particles[i].startLifetime, particles[i].remainingLifetime);
            }
            vanishSystem.SetParticles(particles);
        }
        if (time > timeInterval && !renderer.isVisible)
        {
            time = 0f;
            Vector3 screenPos = camera.WorldToScreenPoint(target.transform.position);
            if (screenPos.z > 0)
            {
                screenPos.x = Mathf.Clamp(screenPos.x, 0, camera.pixelWidth);
            }
            else
            {
                if (screenPos.x < camera.pixelWidth / 2)
                {
                    screenPos.x = camera.pixelWidth;
                }
                else
                {
                    screenPos.x = 0;
                }
            }
            screenPos.y = Mathf.Clamp(screenPos.y, 0, camera.pixelHeight);
            var main = vanishSystem.main;
            if (screenPos.y == 0)
            {
                main.startRotationZ = Mathf.Deg2Rad * 180;
            }
            if (screenPos.y == camera.pixelHeight)
            {
                main.startRotationZ = Mathf.Deg2Rad * 0;
            }
            if (screenPos.x == 0)
            {
                main.startRotationZ = Mathf.Deg2Rad * 270;
            }
            if (screenPos.x == camera.pixelWidth)
            {
                main.startRotationZ = Mathf.Deg2Rad * 90;
            }
            screenPos.z = setZ;
            vanishSystem.transform.position = camera.ScreenToWorldPoint(screenPos);
            vanishSystem.Emit(1);
        }
    }
}
