using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClipTriggerReceiver : MonoBehaviour
{
    public float startTime;
    private float t;
    public UnityEvent eventToCall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        
    }

    public void Entry()
    {
        //Debug.Log(gameObject.name + " Entry");
        if (!gameObject.activeSelf)
        {
            return;
        }

        if(t> startTime)
        {
            t = 0.0f;
            eventToCall.Invoke();
        }
    }
}
