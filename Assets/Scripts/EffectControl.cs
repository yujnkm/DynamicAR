using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EffectControl : MonoBehaviour
{
    public GameObject screen;
    private VideoPlayer danceVideo;
    private VideoPlayer splashVideo;
    private bool flip;
    private bool actionStarted;

    void Start()
    {
        danceVideo = screen.GetComponent<VideoPlayer>();
        splashVideo = GetComponent<VideoPlayer>();
        splashVideo.Play();
        splashVideo.Pause();
    }

    void Update()
    {
        if (danceVideo.isPlaying && !actionStarted)
        {
            StartCoroutine(DelayAction(Random.Range(3, 6)));
        }
    }

    IEnumerator DelayAction(float delayTime)
    {
        actionStarted = true;
        yield return new WaitForSeconds(delayTime);
        transform.position = new Vector3(screen.transform.position.x + Random.Range(0, 1), screen.transform.position.y + Random.Range(0, 1), screen.transform.position.z + Random.Range(0, 1));
        flip = !flip;
        if (flip) transform.Rotate(Vector3.up * 180);
        splashVideo.Play();
        actionStarted = false;

    }
}