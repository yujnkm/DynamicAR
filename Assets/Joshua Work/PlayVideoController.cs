using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideoController : MonoBehaviour
{
    public GameObject dancer;
    private bool isReady = false;

    private void OnEnable()
    {
        isReady = false;
    }
    private void Update()
    {
        Debug.Log("Working");
    }
    public bool getStatus()
    {
        return isReady;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "MainCamera")
        {
            Debug.Log("activate video");
            isReady = true;
        }
    }
}
