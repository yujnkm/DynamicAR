using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public float timeToFade;
    public float withinDist;
    public GameObject spiralSystem;

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
        spiralSystem.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !videoPlayer.isPlaying && isPlayed)
        {
            Debug.Log("clicked");
            fadeIn = true;
            StartCoroutine(PlayDancerVideo());
        }

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

        if (!isPlayed && distance < withinDist)
        {
            isPlayed = true;
            fadeIn = true;
            StartCoroutine(PlayDancerVideo());
        }
    }
    public bool getIsPlayed()
    {
        return isPlayed;
    }
    public GameObject getParent()
    {
        return this.transform.parent.gameObject;
    }
    IEnumerator PlayDancerVideo()
    {
        Debug.Log("awakning spiral");
        videoPlayer.Play();
        yield return new WaitForSeconds((float) videoPlayer.length - timeToFade);
        fadeOut = true;
        yield return new WaitForSeconds(timeToFade);
        if (!spiralSystem.activeSelf)
        {
            spiralSystem.SetActive(true);
            spiralSystem.GetComponent<ParticleSystem>().Play();
            GameObject.FindObjectOfType<IndicatorSystemController>().isDone = true;
        }
    }
}