using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapController : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float startForce;
    public float ceilingCap;
    public float floorCap;
    public float SpeedMax;

    private int maxParticles;
    private ParticleSystem.Particle[] particles;
    private int numParticles;

    void Start()
    {
        maxParticles = particleSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];
    }

    void Update()
    {
        particles = new ParticleSystem.Particle[maxParticles];
        numParticles = particleSystem.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {
            if (particles[i].position.y > ceilingCap)
            {
                Vector3 velocity = particles[i].velocity;
                velocity.y -= (particles[i].position.y - ceilingCap) * startForce * Time.deltaTime;
                particles[i].velocity = velocity;
            }
            if (particles[i].position.y < floorCap)
            {
                Vector3 velocity = particles[i].velocity;
                velocity.y += (floorCap - particles[i].position.y) * startForce * Time.deltaTime;
                particles[i].velocity = velocity;
            }
            if (particles[i].velocity.sqrMagnitude > SpeedMax * SpeedMax)
            {
                particles[i].velocity = particles[i].velocity.normalized * SpeedMax;
            }
        }
        particleSystem.SetParticles(particles);
    }
}
