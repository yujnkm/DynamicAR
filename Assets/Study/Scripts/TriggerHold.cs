using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TriggerHold : MonoBehaviour
{
    // Start is called before the first frame update

    private bool hold;
    private float t;
    public float holdTime = 3.0f;
    public UnityEvent onFinishHolding;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.M)){
            t = 0.0f;
        }

        if (Input.GetKey(KeyCode.M)){
            t += Time.deltaTime;

            if (t > holdTime) {
                t = 0.0f;
                onFinishHolding.Invoke();
            }
        }

   
    }
}
