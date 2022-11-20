using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSystemController : MonoBehaviour
{
    public ParticleSystem vanishSystem;
    public Camera camera;
    public GameObject targetObject;
    public GameObject targetSwitch;
    public float setZ;
    public float timeInterval;
    public int maxParticles;

    private Renderer renderer;
    private float time;

    void Start()
    {
        targetObject = GameObject.FindGameObjectWithTag("Dancer");
        targetSwitch = GameObject.FindGameObjectWithTag("Target");
        time = 0f;
    }

    void Update()
    {
        /*
        GameObject nextTarget = GameObject.FindObjectOfType<IndicatorSystemController>().isDone ? targetSwitch : targetObject;
        renderer = nextTarget.GetComponent<Renderer>();
        time += Time.deltaTime;
        Debug.Log(renderer.isVisible);
        if (time > timeInterval && !renderer.isVisible)
        {
            time = 0f;
            Vector3 screenPos = camera.WorldToScreenPoint(nextTarget.transform.position);
            if (screenPos.z > 0)
            {
                screenPos.x = Mathf.Clamp(screenPos.x, 0, camera.pixelWidth);
            }
            else
            {
                if (screenPos.x < camera.pixelWidth / 2)
                {
                    screenPos.x = camera.pixelWidth;
                }
                else
                {
                    screenPos.x = 0;
                }
            }
            screenPos.y = Mathf.Clamp(screenPos.y, 0, camera.pixelHeight);
            screenPos.z = setZ;
            vanishSystem.transform.position = camera.ScreenToWorldPoint(screenPos);
            vanishSystem.Emit(1);
        }
        */
    }
}
