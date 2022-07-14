using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    public Canvas canvas;
    public Image image;
    public bool fadeOut;
    public float timeToFade;
    
    private bool fadeIn;
    private float timeIn;
    private float timeOut;

    void Start()
    {
        fadeOut = false;
        fadeIn = true;
        timeIn = 0f;
        timeOut = 0f;
    }

    void Update()
    {
        if (fadeIn)
        {
            timeIn += Time.deltaTime;
            Color color = image.color;
            color.a = color.a - 1f / timeToFade * Time.deltaTime;
            if (color.a < 0)
            {
                color.a = 0;
                fadeIn = false;
            }
            image.color = color;
        }
        if (fadeOut)
        {
            timeOut += Time.deltaTime;
            Color color = image.color;
            color.a = color.a + 1f / timeToFade * Time.deltaTime;
            if (color.a > 1)
            {
                color.a = 1;
                fadeOut = false;
            }
            image.color = color;
        }
    }
}
