using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool isPlayed = false;
    private Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        videoPlayer.Play();
        videoPlayer.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        Debug.Log(renderer.isVisible);

        if (!isPlayed && distance < 10)
        {
            GetComponent<MeshRenderer>().enabled = true;
            if(renderer.isVisible)
            {
                videoPlayer.Play();
                isPlayed = true;
            }
            
        }
        else if(!isPlayed)
            GetComponent<MeshRenderer>().enabled = false;

    }
}