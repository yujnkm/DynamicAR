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
    public float fadeTime;
    public GameObject activator;

    private VideoPlayer videoPlayer;
    private bool fadeIn;
    private bool fadeOut;
    private Renderer _renderer;
    private VideoActivator videoActivator;
    private Transform[] specialEffect;
    private const int DEFAULT = 0;
    private const int THEATER = 3;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    void OnEnable()
    {
        fadeIn = false;
        fadeOut = false;
        Color color = _renderer.material.color;
        color.a = 0f;
        _renderer.material.color = color;

        //video can't collide with particle unless it is the target
        this.gameObject.layer = DEFAULT;

        TutorialController.Instance.incrementTotalDancers();
    }
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoActivator = activator.GetComponent<VideoActivator>();
        specialEffect = this.gameObject.GetComponentsInChildren<Transform>();
    }
    void Update()
    {
        /*
         * users can use remote to replay video if within activator box
         * checks if 1) user clicked button 2) video played once already 3) user is within box
         */
        if (Input.GetKeyDown(KeyCode.V) && videoActivator.GetActivationStatus() && videoActivator.IsWithin())
        {
            fadeIn = true;
            StartCoroutine(PlayDancerVideo());
        }

        //plays video when "activator" is activated
        if (CheckPlay())
        {
            fadeIn = true;
            TutorialController.Instance.incrementViewedDancers();
            StartCoroutine(PlayDancerVideo());
        }

        /*
         * fade transitions
         * currently won't show up due to cutout material
         * using cutout instead of fade material for optimization
         */
        if (fadeIn)
        {
            Color color = _renderer.material.color;
            color.a = color.a + 1f / fadeTime * Time.deltaTime;
            if (color.a > 1)
            {
                color.a = 1;
                fadeIn = false;
            }
            _renderer.material.color = color;
        }
        if (fadeOut)
        {
            Color color = _renderer.material.color;
            color.a = color.a - 1f / fadeTime * Time.deltaTime;
            if (color.a < 0)
            {
                color.a = 0;
                fadeOut = false;
            }
            _renderer.material.color = color;
        }
    }
    public bool CheckPlay()
    {
        return videoActivator.GetReadyStatus();
    }
    /*
     * fade out arrow
     * fade in dancer video and plays
     * plays special effects attached to dancer video
     */
    IEnumerator PlayDancerVideo()
    {
        videoActivator.Completed();
        ArrowController.Instance.ArrowFadeOut();
        videoPlayer.Play();

        /*
         * activates special effects
         * currently accounts of videos, particle systems, and audio
         */
        foreach (Transform effect in specialEffect)
        {
            if (effect.gameObject.GetComponent<VideoPlayer>() != null)
            {
                effect.gameObject.GetComponent<VideoPlayer>().Play();
            }
            if (effect.gameObject.GetComponent<ParticleSystem>() != null)
            {
                effect.gameObject.GetComponent<ParticleSystem>().Play();
            }
            if (effect.gameObject.GetComponent<AudioSource>() != null)
            {
                effect.gameObject.GetComponent <AudioSource>().Play();
            }
        }
        yield return new WaitForSeconds((float) videoPlayer.length - fadeTime);
        fadeOut = true;
        yield return new WaitForSeconds(fadeTime);

        //stops special effects
        foreach (Transform effect in specialEffect)
        {
            if (effect.gameObject.GetComponent<VideoPlayer>() != null)
            {
                effect.gameObject.GetComponent<VideoPlayer>().Stop();
            }
            if (effect.gameObject.GetComponent<ParticleSystem>() != null)
            {
                effect.gameObject.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            if (effect.gameObject.GetComponent<AudioSource>() != null)
            {
                effect.gameObject.GetComponent<AudioSource>().Stop();
            }
        }
        ArrowController.Instance.ArrowFadeIn();
    }
}