using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialAudioTask : MonoBehaviour
{
    public bool giveFeedback=true;
    public List<AudioClip> clips;
    public int correctClipIndex;
    private int currentIndex;

    public AudioClip correct;
    public AudioClip wrong;

    public int correctAnswers;
    public int goal;
    private float t = 0.0f;
    public float timeBetween;

    public AudioSource source;
    public UnityEvent onCompleted;
    private TutorialController controller;

    public int[] playlist;
    public bool isRandom;
    private int loc;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("TutorialController").GetComponent<TutorialController>();
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > timeBetween)
        {
            t = 0;
            timeBetween = Random.Range(2f, 5f);
            PlayNext();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            bool corr = false;
            if(currentIndex == correctClipIndex)
            {
                corr = true;
                if (giveFeedback)
                {
                    source.PlayOneShot(correct);
                }
                
                correctAnswers++;
                if (correctAnswers >= goal)
                {
                    onCompleted.Invoke();
                }
            }
            else
            {
                if (giveFeedback)
                {
                    source.PlayOneShot(wrong);
                }
                
            }

            string line = "audio, " + Time.time.ToString("0.00") + ", " + corr + ", " + currentIndex + ", " + correctClipIndex;
            controller.WriteLine(line);

        }
    }

    public void PlayNext()
    {
        if (isRandom)
        {
            currentIndex = Random.Range(0, clips.Count);
        }
        else
        {
            if (loc< playlist.Length - 1)
            {
                loc++;
                
            }
            else
            {
                loc = 0;
            }

            currentIndex = playlist[loc];
        }

        source.PlayOneShot(clips[currentIndex]);
    }
}
