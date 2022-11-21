using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralPositionController : MonoBehaviour
{
    void OnEnable()
    {
        SwitchController.Instance.transform.position = transform.position;
    }
}
