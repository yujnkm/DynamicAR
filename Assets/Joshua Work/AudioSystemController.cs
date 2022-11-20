using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystemController : MonoBehaviour
{
    public GameObject[] audioClips;
    public int startClip;
    public int endClip;

    private bool playing = false;

    void Update()
    {
        /*
         * Audio will start playing at "startClip" scene and end at "endClip" scene
         * Created Singleton in TutorialController for quick access
         */
        if (TutorialController.Instance.currentClip == startClip && !playing)
        {
            //Plays all clips in "audioClip"
            playing = true;
            foreach (GameObject audio in audioClips)
            {
                audio.GetComponent<AudioSource>().Play();
            }
        }
        if (TutorialController.Instance.currentClip == endClip && playing)
        {
            playing = false;
            foreach (GameObject audio in audioClips)
            {
                audio.GetComponent<AudioSource>().Stop();
            }
        }
    }
}
