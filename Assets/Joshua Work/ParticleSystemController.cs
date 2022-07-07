using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public ParticleSystem particleSystem;
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
        maxParticles = particleSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];
        collisions = new List<ParticleCollisionEvent>();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= timeChange)
        {
            numParticles = particleSystem.GetParticles(particles);
            time = 0f;
            for (int i = 0; i < numParticles; i++)
            {
                Vector3 direction = (target.transform.position - particles[i].position).normalized;
                particles[i].velocity = direction * driftSpeed;
            }
            particleSystem.SetParticles(particles);
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
        ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, collisions);
        for (int i = 0; i < collisions.Count; i++)
        {
            numParticles = particleSystem.GetParticles(particles);
            int index = findParticleIndex(collisions[i].intersection);
            particles[index].position = new Vector3(INFINITY, INFINITY, INFINITY);
            particleSystem.SetParticles(particles);
            explosionSystem.transform.position = collisions[i].intersection;
            explosionSystem.transform.rotation = Quaternion.LookRotation(collisions[i].normal);
            explosionSystem.Play();
        }
    }
}
