using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Vector3 pos;
    public float distance = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Align();
    }

    public void Align()
    {
        pos = transform.position;
        Transform cam = Camera.main.transform;
        transform.position = new Vector3(cam.forward.x, 0, cam.forward.z).normalized * distance + cam.position;
        transform.LookAt(cam, Vector3.up);
        Vector3 euler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(0, euler.y, 0);
    }
}
