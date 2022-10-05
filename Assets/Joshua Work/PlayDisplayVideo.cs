using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/*
 * different from playVideo
 * PlayDisplayVideo is for the 2 secondary dancer videos that
 * do not activate the spiral system
 * while playVideo is for the main dancer screen
 */
public class PlayDisplayVideo : MonoBehaviour
{
    public float timeToFade;
    public float withinDist;

    private VideoPlayer videoPlayer;
    private bool isPlayed;
    private bool fadeIn;
    private float timeIn;
    private bool fadeOut;
    private float timeOut;
    private Renderer renderer;

    void OnEnable()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        color.a = 0f;
        renderer.material.color = color;
        isPlayed = false;
        fadeIn = false;
        timeIn = 0f;
        fadeOut = false;
        timeOut = 0f;
    }

    void Update()
    {
        /*
         * fades video in and out when they play
         */
        if (fadeIn)
        {
            timeIn += Time.deltaTime;
            Color color = renderer.material.color;
            color.a = color.a + 1f / timeToFade * Time.deltaTime;
            //color.a = color.a + 1f;
            if (color.a > 1)
            {
                color.a = 1;
                fadeIn = false;
            }
            renderer.material.color = color;
        }
        if (fadeOut)
        {
            timeIn += Time.deltaTime;
            Color color = renderer.material.color;
            color.a = color.a - 1f / timeToFade * Time.deltaTime;
            //color.a = color.a - 1f;
            if (color.a < 0)
            {
                color.a = 0;
                fadeOut = false;
            }
            renderer.material.color = color;
        }

        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        /*
         * plays video when user is a certain distance away from the dancers
         */
        if (!isPlayed && distance < withinDist)
        {
            isPlayed = true;
            fadeIn = true;
            StartCoroutine(PlayDancerVideo());
        }
    }
    IEnumerator PlayDancerVideo()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds((float)videoPlayer.length - timeToFade);
        fadeOut = true;
        yield return new WaitForSeconds(timeToFade);
    }
}