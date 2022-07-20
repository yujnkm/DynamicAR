using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ArrowController : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject arrowLocation;
    public float rotateSpeed;
    public float amplitude;
    public float frequency;
    public float fadeTime;

    private VanishSystemController vanishSystemController;
    private bool fadeIn;
    private bool fadeOut;
    private Renderer renderer;

    void Start()
    {
        vanishSystemController = GameObject.FindObjectOfType<VanishSystemController>();
        fadeIn = false;
        fadeOut = false;
        renderer = transform.GetChild(0).GetComponent<Renderer>();
    }

    void Update()
    {
        transform.position = new Vector3(arrowLocation.transform.position.x, transform.position.y, arrowLocation.transform.position.z);
        targetObject = vanishSystemController.nextTarget;
        Vector3 targetPosition = new Vector3(targetObject.transform.position.x, transform.position.y, targetObject.transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed);

        Vector3 position = transform.position;
        position += transform.forward * Mathf.Sin(frequency * Time.time) * amplitude;
        transform.position = position;

        if (targetObject.tag == "Dancer" && targetObject.transform.GetChild(0).GetComponent<VideoPlayer>().isPlaying)
        {
            fadeOut = true;
        }
        else
        {
            fadeIn = true;
        }

        if (fadeIn)
        {
            Color color = renderer.material.color;
            color.a = color.a + 1f / fadeTime * Time.deltaTime;
            if (color.a > 1)
            {
                color.a = 1;
                fadeIn = false;
            }
            renderer.material.color = color;
        }
        if (fadeOut)
        {
            Color color = renderer.material.color;
            color.a = color.a - 1f / fadeTime * Time.deltaTime;
            if (color.a < 0)
            {
                color.a = 0;
                fadeOut = false;
            }
            renderer.material.color = color;
        }
    }
}
