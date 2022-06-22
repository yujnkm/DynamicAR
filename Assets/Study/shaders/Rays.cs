using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rays : MonoBehaviour
{
    public Material mat;
    private Vector2 offset;
    public float speed = 0.05f;
    public float init;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        offset.x = init;
    }

    // Update is called once per frame
    void Update()
    {
        offset.x = Mathf.Sin(init+Time.time*speed);
        mat.SetTextureOffset("_MainTex", offset);
    }
}
