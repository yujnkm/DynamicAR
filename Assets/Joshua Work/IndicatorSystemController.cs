using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorSystemController : MonoBehaviour
{
    public ParticleSystem indicatorSystem;
    public ParticleSystem explosionSystem;
    public GameObject targetObject;
    public float driftSpeed;
    public float timeChange;
    public float closeEnough;
    public int explosionEmit;

    [Header("Optional Target")]
    [Tooltip("Leave as \"none\" if targetObject has collider" )]
    public GameObject actualObject;

    private float time = 0f;
    private int numParticles;
    private ParticleSystem.Particle[] particles;
    private List<ParticleCollisionEvent> collisions;
    private int maxParticles;
    private float initialSize;
    private float sizeChange = 0.05f;
    private const float EPSILON = 0.001f;

    #region Singleton
    public static IndicatorSystemController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    void OnEnable()
    {
        maxParticles = indicatorSystem.main.maxParticles;
        initialSize = indicatorSystem.main.startSize.constant;
        particles = new ParticleSystem.Particle[maxParticles];
        collisions = new List<ParticleCollisionEvent>();
    }
    void Update()
    {
        time += Time.deltaTime;
        if (time > timeChange)
        {
            particles = new ParticleSystem.Particle[maxParticles];
            numParticles = indicatorSystem.GetParticles(particles);
            time = 0f;
            for (int i = 0; i < numParticles; i++)
            {
                Vector3 direction = (targetObject.transform.position - particles[i].position).normalized;

                /*
                 * if actualObject has no collider, particles must go to an object that has collider
                 * useful for dancers' activation points, which have no collider
                 */
                if (actualObject != null)
                {
                    if (Vector3.Distance(particles[i].position, targetObject.transform.position) < closeEnough)
                    {
                        /*
                         * if particle has reached the initial target, the start size will decrease by a tiny bit (0.05)
                         * because particles don't have individual data, the change in start size will act as a boolean
                         */
                        if (Mathf.Abs(initialSize - particles[i].startSize) < EPSILON)
                        {
                            particles[i].startSize = particles[i].startSize - sizeChange;
                        }
                    }
                    //if particle size has been modified, that means particle must switch to new target
                    if (Mathf.Abs(particles[i].startSize - (initialSize - sizeChange)) < EPSILON)
                    {
                        direction = (actualObject.transform.position - particles[i].position).normalized;
                    }
                }

                /*
                 * propels particles in correct direction of dancer
                 * necessary since particles have a randomizing effect on their velocity
                 */
                particles[i].velocity = direction * driftSpeed;
            }
            indicatorSystem.SetParticles(particles);
        }
    }
    public void ActivateIndicators()
    {
        this.gameObject.GetComponent<ParticleSystem>().Play();
    }
    public void StopIndicators()
    {
        this.gameObject.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    //finds which particle is closest to a given position
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
    //when particles collide with the dancer screen
    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(indicatorSystem, other, collisions);
        for (int i = 0; i < collisions.Count; i++)
        {
            particles = new ParticleSystem.Particle[maxParticles];
            numParticles = indicatorSystem.GetParticles(particles);

            //find out which particle is the one that collided
            int index = FindParticleIndex(collisions[i].intersection);

            //destroys the particle
            particles[index].remainingLifetime = 0f;
            indicatorSystem.SetParticles(particles);

            //plays a small explosion effect when particles reach the dancer object
            explosionSystem.transform.position = collisions[i].intersection;
            explosionSystem.transform.rotation = Quaternion.LookRotation(collisions[i].normal);
            explosionSystem.Play();
        }
    }
}
