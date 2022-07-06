using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collided {collision.gameObject.name}");
    }
}
