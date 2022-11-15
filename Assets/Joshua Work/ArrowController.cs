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

    void OnEnable()
    {
        vanishSystemController = GameObject.FindObjectOfType<VanishSystemController>();
        fadeIn = false;
        fadeOut = false;
        renderer = transform.GetChild(0).GetComponent<Renderer>();
        targetObject = null;
    }

    void Update()
    {
        transform.position = new Vector3(arrowLocation.transform.position.x, transform.position.y, arrowLocation.transform.position.z);
        //targetObject = vanishSystemController.nextTarget;
        PlayVideo playVideoComponent = GameObject.FindGameObjectWithTag("MainDance").GetComponent<PlayVideo>();
        targetObject = playVideoComponent.getParent();
        if (playVideoComponent.getIsPlayed() && !targetObject.transform.GetChild(0).GetComponent<VideoPlayer>().isPlaying)
        {
            targetObject = GameObject.FindGameObjectWithTag("Target");
        }
        /*
         * Finds position of the target and rotate towards it
         */
        Vector3 targetPosition = new Vector3(targetObject.transform.position.x, transform.position.y, targetObject.transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed);

        /*
         * Arrow oscillates back and forth (just a visual effect)
         */
        Vector3 position = transform.position;
        position += transform.forward * Mathf.Sin(frequency * Time.time) * amplitude;
        transform.position = position;

        /*
         * Arrow disappears when the main dancer video is playing
         * Only appears when dancers are not playing or when dancers are done playing
         */
        if (targetObject.tag == "Dancer" && targetObject.transform.GetChild(0).GetComponent<VideoPlayer>().isPlaying)
        {
            Debug.Log("Dancer is playing");
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
            Debug.Log(color.a);
            if (color.a < 0)
            {
                color.a = 0;
                fadeOut = false;
            }
            renderer.material.color = color;
        }
    }
}
