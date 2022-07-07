using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkSystemManager : MonoBehaviour
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
        time += Time.deltaTime;
        if (time > timeInterval)
        {
            time = 0;
            fireworkSystems[(int)Random.Range(0f, fireworkSystems.Length)].Emit(1);
        }
    }
}