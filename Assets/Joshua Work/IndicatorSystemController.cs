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

    void OnEnable()
    {
        maxParticles = indicatorSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];
        collisions = new List<ParticleCollisionEvent>();
        isDone = false;
        targetObject = null;
        targetSwitch = null;
    }
    void Update()
    {
        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Dancer"); //finds the dancer object
        }
        if (targetSwitch == null)
        {
            targetSwitch = GameObject.FindGameObjectWithTag("Target"); //finds the spiral object
        }
        time += Time.deltaTime;
        if (time >= timeChange)
        {
            particles = new ParticleSystem.Particle[maxParticles];
            numParticles = indicatorSystem.GetParticles(particles);
            time = 0f;
            for (int i = 0; i < numParticles; i++)
            {
                /*
                 * if the dancer video did not play or is not done playing, the indicators will go to the dancers.
                 * if the dancer video is completed, then the indicators will go to the spiral to prompt users to move to next scene
                 */
                GameObject nextTarget = isDone ? targetSwitch : targetObject;
                Vector3 direction = (nextTarget.transform.position - particles[i].position).normalized;
                /*
                 * propels particles in correct direction of dancer
                 * necessary since particles have a randomizing effect on their velocity
                 */
                particles[i].velocity = direction * driftSpeed;
            }
            indicatorSystem.SetParticles(particles);
        }
    }
    /*
     * finds which particle is closest to a given position
     */
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
    private void OnParticleCollision(GameObject other) //when particles collide with the dancer screen
    {
        ParticlePhysicsExtensions.GetCollisionEvents(indicatorSystem, other, collisions);
        for (int i = 0; i < collisions.Count; i++)
        {
            particles = new ParticleSystem.Particle[maxParticles];
            numParticles = indicatorSystem.GetParticles(particles);
            int index = FindParticleIndex(collisions[i].intersection); //find out which particle is the one that collided
            particles[index].remainingLifetime = 0f; //destroys the particle
            indicatorSystem.SetParticles(particles);
            /*
             * plays a small explosion effect when particles reach the dancer object
             */
            explosionSystem.transform.position = collisions[i].intersection;
            explosionSystem.transform.rotation = Quaternion.LookRotation(collisions[i].normal);
            explosionSystem.Play();
        }
    }
}
