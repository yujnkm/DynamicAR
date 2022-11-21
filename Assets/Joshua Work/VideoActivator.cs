using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoActivator : MonoBehaviour
{
    public GameObject dancer;

    private bool isReady;
    private bool activated;
    private bool within;

    private void OnEnable()
    {
        isReady = false;
        activated = false;
        within = false;
    }
    public bool GetReadyStatus()
    {
        return isReady;
    }
    public void Completed()
    {
        isReady = false;
    }
    public bool GetActivationStatus()
    {
        return activated;
    }
    public bool IsWithin()
    {
        return within;
    }
    private void OnTriggerEnter(Collider other)
    {
        //each video will only activate automatically once
        if (other.gameObject.tag == "MainCamera")
        {
            within = true;
            if (!activated)
            {
                //marks object as found in the list
                TargetObjectController.Instance.FindAndMarkObject(this.gameObject);

                isReady = true;
                activated = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        within = false;
    }
}
