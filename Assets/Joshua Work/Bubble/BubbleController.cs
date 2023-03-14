using Microsoft.MixedReality.Toolkit.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    public float sizeRadius;
    public float speed;
    public float timeChange;
    public GameObject center;

    private AudioSource audioSource;
    private float time;

    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(sizeRadius, sizeRadius, sizeRadius);
        audioSource = GetComponent<AudioSource>();
        audioSource.minDistance = 0;
        audioSource.maxDistance = sizeRadius / 2;
        time = 0;

        transform.RotateAround(transform.position, transform.up, Random.Range(0, 360));
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (time > timeChange)
        {
            transform.RotateAround(transform.position, transform.up, Random.Range(-135, 135));
            time = 0;
        }
        time += Time.deltaTime;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bounds")
        {
            Quaternion rotation = Quaternion.LookRotation(center.transform.position - this.transform.position, Vector3.up);
            this.gameObject.transform.rotation = rotation;
            time = 0;
        }
    }
}
