using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyToAdvance : MonoBehaviour
{
    public KeyCode key;
    public float timer;
    private float t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (t > timer)
        {
            if (Input.GetKeyDown(key))
            {
                gameObject.SendMessage("Done");
            }
        }
    }
}
