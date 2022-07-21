using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialClip : MonoBehaviour
{
    //public AudioClip clip;
    public AudioClip[] clips;
    private AudioSource source;
    private TutorialController controller;
    public bool hasTimer;
    public bool endWithAudio;

    public float time;

    public float timedObjectTime =0.0f;
    public GameObject timedObject;
    private float t;
    private float totalAudioTime = 0f;

    public bool anyKeytoAdvance = false;

    public bool setAsCurrent = false;
    public float anyKeyMinTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        totalAudioTime = 0f;
        controller = transform.parent.gameObject.GetComponent<TutorialController>();
        source = controller.gameObject.GetComponent<AudioSource>();
        
        for (var i = 0; i < clips.Length; i++)
        {
            Debug.Log(clips[i].length);
            totalAudioTime += clips[i].length;
        }
    }

    public void Ready()
    {
        Start();
    }
    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t>time&& hasTimer)
        {
            if (timedObjectTime > 0.01f)
            {
                if(t > timedObjectTime)
                {
                    timedObjectTime = 0.0f;
                    if (timedObject != null)
                    {
                        timedObject.SetActive(true);
                    }
                }
            }

            t = 0;
            Done();
        }

        if (endWithAudio)
        {
            if (t > totalAudioTime + 0.3f)
            {
                t = 0.0f;
                Done();
            }
        }

        if(anyKeytoAdvance && Input.anyKeyDown && !Input.GetMouseButtonDown(1)){
            if(t> anyKeyMinTime)
            {
                Done();
            }
            
        }
    }

    public void Play()
    {
        StartCoroutine(PlayClips());
    }

    IEnumerator PlayClips()
    {
        for(var i = 0; i < clips.Length; i++)
        {
            source.clip = clips[i];
            source.Play();
            yield return new WaitForSeconds(clips[i].length);
        }
        yield return null;
    }

    public void OnValidate()
    {
        if (setAsCurrent)
        {
            controller = transform.parent.gameObject.GetComponent<TutorialController>();
            controller.SetClip(this);
            setAsCurrent = false;
        }
    }
    public void Done()
    {
        if (gameObject.activeSelf)
        {
            controller.Next();
        }        
    }
    public void JumpTo()
    {
        if (gameObject.activeSelf)
        {
            controller.JumpScene();
        }
    }
    public void MoveBack()
    {
        controller.Previous();
    }
}
