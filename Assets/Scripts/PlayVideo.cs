using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public float timeToFade;
    public GameObject spiralSystem;

    private VideoPlayer videoPlayer;
    private bool isPlayed;
    private bool fadeIn;
    private float timeIn;
    private bool fadeOut;
    private float timeOut;
    private Renderer renderer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        isPlayed = false;
        fadeIn = false;
        timeIn = 0f;
        fadeOut = false;
        timeOut = 0f;
        spiralSystem.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            fadeIn = true;
            isPlayed = true;
            StartCoroutine(PlayDancerVideo());
        }

        Debug.Log(renderer.isVisible);

        if (fadeIn)
        {
            timeIn += Time.deltaTime;
            Color color = renderer.material.color;
            color.a = color.a + 1f / timeToFade * Time.deltaTime;
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
            if (color.a < 0)
            {
                color.a = 0;
                fadeOut = false;
            }
            renderer.material.color = color;
        }

        if (!isPlayed && distance < 10)
        {
            isPlayed = true;
            StartCoroutine(PlayDancerVideo());
            fadeIn = true;
        }
    }
    IEnumerator PlayDancerVideo()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds((float) videoPlayer.length - timeToFade);
        fadeOut = true;
        yield return new WaitForSeconds(timeToFade);
        if (!spiralSystem.activeSelf)
        {
            spiralSystem.SetActive(true);
            spiralSystem.GetComponent<ParticleSystem>().Play();
            IndicatorSystemController.isDone = true;
        }
    }
}