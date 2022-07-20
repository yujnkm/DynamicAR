using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorSystemController : MonoBehaviour
{
    public ParticleSystem indicatorSystem;
    public ParticleSystem explosionSystem;
    public GameObject targetObject;
    public GameObject targetSwitch;
    public float driftSpeed;
    public float timeChange;
    public int explosionEmit;
    public bool isDone;

    private float time = 0f;
    private int numParticles;
    private ParticleSystem.Particle[] particles;
    private List<ParticleCollisionEvent> collisions;
    private int maxParticles;
    private int INFINITY = 1_000_000;

    void Start()
    {
        maxParticles = indicatorSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];
        collisions = new List<ParticleCollisionEvent>();
        isDone = false;
        targetObject = GameObject.FindGameObjectWithTag("Dancer");
    }
    void Update()
    {
        if (targetSwitch == null)
        {
            targetSwitch = GameObject.FindGameObjectWithTag("Target");
        }
        time += Time.deltaTime;
        if (time >= timeChange)
        {
            particles = new ParticleSystem.Particle[maxParticles];
            numParticles = indicatorSystem.GetParticles(particles);
            time = 0f;
            for (int i = 0; i < numParticles; i++)
            {
                GameObject nextTarget = isDone ? targetSwitch : targetObject;
                Vector3 direction = (nextTarget.transform.position - particles[i].position).normalized;
                particles[i].velocity = direction * driftSpeed;
            }
            indicatorSystem.SetParticles(particles);
        }
    }
    private int FindParticleIndex(Vector3 position)
    {
        float minDist = float.MaxValue;
        int closest = 0;
        for (int i = 0; i < maxParticles; i++)
        {
            ParticleSystem.Particle cur = particles[i];
            float curDist = (cur.position - position).sqrMagnitude;
            if (curDist < minDist)
            {
                minDist = curDist;
                closest = i;
            }
        }
        return closest;
    }
    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(indicatorSystem, other, collisions);
        for (int i = 0; i < collisions.Count; i++)
        {
            particles = new ParticleSystem.Particle[maxParticles];
            numParticles = indicatorSystem.GetParticles(particles);
            int index = FindParticleIndex(collisions[i].intersection);
            particles[index].remainingLifetime = 0f;
            indicatorSystem.SetParticles(particles);
            explosionSystem.transform.position = collisions[i].intersection;
            explosionSystem.transform.rotation = Quaternion.LookRotation(collisions[i].normal);
            explosionSystem.Play();
        }
    }
}
