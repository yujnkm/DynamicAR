using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Blinking : MonoBehaviour
{
    public float speed;
    private Image image;
    public Color col;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        col.a = (Mathf.Sin(Time.time * speed) + 1.0f) / 2f;
        image.color = col;
    }
}
