using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkSystemController : MonoBehaviour
{
    public ParticleSystem[] fireworkSystems;
    public float timeInterval;

    private float time;

    void Start()
    {
        time = 0f;
    }
    void Update()
    {
        /*
         * chooses a random firework with a random color to emit after a certain amount of time
         */
        time += Time.deltaTime;
        if (time > timeInterval)
        {
            time = 0;
            fireworkSystems[(int)Random.Range(0f, fireworkSystems.Length)].Emit(1);
        }
    }
}