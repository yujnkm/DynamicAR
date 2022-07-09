using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorSystemController : MonoBehaviour
{
    public ParticleSystem indicatorSystem;
    public ParticleSystem explosionSystem;
    public GameObject target;
    public float driftSpeed;
    public float timeChange;
    public int explosionEmit;

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
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= timeChange)
        {
            numParticles = indicatorSystem.GetParticles(particles);
            time = 0f;
            for (int i = 0; i < numParticles; i++)
            {
                Vector3 direction = (target.transform.position - particles[i].position).normalized;
                particles[i].velocity = direction * driftSpeed;
            }
            indicatorSystem.SetParticles(particles);
        }
    }
    private int findParticleIndex(Vector3 position)
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
            numParticles = indicatorSystem.GetParticles(particles);
            int index = findParticleIndex(collisions[i].intersection);
            particles[index].position = new Vector3(INFINITY, INFINITY, INFINITY);
            indicatorSystem.SetParticles(particles);
            explosionSystem.transform.position = collisions[i].intersection;
            explosionSystem.transform.rotation = Quaternion.LookRotation(collisions[i].normal);
            explosionSystem.Play();
        }
    }
}
