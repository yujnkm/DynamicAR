using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EffectControl : MonoBehaviour
{
    public GameObject screen;
    public float boxIn;
    public float floorHeight;

    private VideoPlayer danceVideo;
    private VideoPlayer splashVideo;
    private bool flip;
    private bool actionStarted;
    private Renderer renderer;

    void Start()
    {
        danceVideo = screen.GetComponent<VideoPlayer>();
        splashVideo = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        splashVideo.Play();
        splashVideo.Pause();
    }

    void Update()
    {
        if (!splashVideo.isPlaying)
        {
            Color color = renderer.material.color;
            color.a = 0f;
            renderer.material.color = color;
        }
        if (danceVideo.isPlaying && !actionStarted)
        {
            StartCoroutine(DelayAction(Random.Range(3, 6)));
        }
    }

    IEnumerator DelayAction(float delayTime)
    {
        actionStarted = true;
        yield return new WaitForSeconds(delayTime);
        Color color = renderer.material.color;
        color.a = 1f;
        renderer.material.color = color;
        var boundingBox = screen.GetComponent<Renderer>().bounds;
        Vector3 lower = boundingBox.min + Vector3.one * boxIn;
        Vector3 upper = boundingBox.max - Vector3.one * boxIn;
        transform.position = new Vector3(Random.Range(lower.x, upper.x), Random.Range(Mathf.Max(lower.y, floorHeight), upper.y), Random.Range(lower.z, upper.z));
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Debug.Log(danceVideo.length - danceVideo.time);
        if (splashVideo.length < danceVideo.length - danceVideo.time)
        {
            splashVideo.Play();
        }
        actionStarted = false;
    }
}