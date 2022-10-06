using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/*
 * PlayVideo is for the main dancer footage
 * Different from PlayDisplayVideo, which is only for the 2 secondary footages
 */
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
    private GameObject videoControl;
    private GameObject[] positionControls;

    void OnEnable()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        videoControl = GameObject.FindGameObjectWithTag("PositionController");
        findPlayPoints();
        Color color = renderer.material.color;
        color.a = 0f;
        renderer.material.color = color;
        isPlayed = false;
        fadeIn = false;
        timeIn = 0f;
        fadeOut = false;
        timeOut = 0f;
        /*
         * spiral is deactivated to not confuse the user
         * will be activated once the user finishes watching the main dancer footage (located at the end of the hall)
         */
        spiralSystem.SetActive(false);
    }

    void Update()
    {
        /*
         * Not used in study
         * Initially designed to allow user to manually play the dancer video,
         * but users did not have remote to do so
         */
        if (Input.GetKeyDown(KeyCode.V) && !videoPlayer.isPlaying && isPlayed)
        {
            Debug.Log("clicked");
            fadeIn = true;
            StartCoroutine(PlayDancerVideo());
        }

        //fades in and out the dancer video when appropriate
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
        if (!isPlayed && checkPlay())
        {
            isPlayed = true;
            fadeIn = true;
            StartCoroutine(PlayDancerVideo());
        }
    }
    private void findPlayPoints()
    {
        positionControls = new GameObject[videoControl.transform.childCount];
        for (int i = 0; i < positionControls.Length; i++)
        {
            positionControls[i] = videoControl.transform.GetChild(i).gameObject;
        }
    }
    public bool checkPlay()
    {
        foreach (GameObject position in positionControls)
        {
            PlayVideoController videoController = position.GetComponent<PlayVideoController>();
            if (videoController.dancer == this.gameObject)
            {
                return videoController.getStatus();
            }
        }
        return false;
    }
    public bool getIsPlayed()
    {
        return isPlayed;
    }
    public GameObject getParent()
    {
        return this.transform.parent.gameObject;
    }
    /*
     * Fades in the dancer video and then plays it
     * Once the video is done playing, the spiral system will activate,
     * and it has a collider that will allow users to jump to an empty scene,
     * where they will fill out the survey before moving on to the next scene
     */
    IEnumerator PlayDancerVideo()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds((float) videoPlayer.length - timeToFade);
        fadeOut = true;
        yield return new WaitForSeconds(timeToFade);
        if (!spiralSystem.activeSelf && this.gameObject.tag == "MainDance")
        {
            spiralSystem.SetActive(true);
            spiralSystem.GetComponent<ParticleSystem>().Play();
            GameObject.FindObjectOfType<IndicatorSystemController>().isDone = true;
        }
    }
}