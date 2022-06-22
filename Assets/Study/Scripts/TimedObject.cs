using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObject : MonoBehaviour
{
    public float timer = 0f;
    public GameObject nextObject;
    private float t;
    public bool deactivateThis = true;
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
            t = 0;
            if (nextObject)
            {     
                nextObject.SetActive(true);
            }
            if (deactivateThis)
            {
                gameObject.SetActive(false);
            }
            
        }
        
    }
}
