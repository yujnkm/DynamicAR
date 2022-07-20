using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSide : MonoBehaviour
{
    public float amplitude;
    public float frequency;

    void Update()
    {
        var rotation = transform.localEulerAngles;
        rotation.z = Mathf.Sin(frequency * Time.time) * amplitude;
        transform.localEulerAngles = rotation;
    }
}
