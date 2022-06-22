using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Highlighter : MonoBehaviour
{
    private bool auto=true;
    public float starttime;
    public float duration;
    public bool changeshader;
    public bool changeColor;
    public bool changeUI;
    public Material mat;
    public Color color;

    private Color lastColor;
    private Shader lastShader;

    public GameObject target;

    private GameObject highlightObject;
    private Renderer[] renderers;
    private MaskableGraphic[] graphics;
    private bool isOn;
    private float timer;


    // Start is called before the first frame update
    void Start()
    {

        if (changeshader)
        {
            highlightObject = GameObject.Instantiate(target,target.transform.position,target.transform.rotation,target.transform.parent);
            highlightObject.AddComponent<Rays>();
            renderers = highlightObject.GetComponentsInChildren<Renderer>();
            foreach(var r in renderers){
                r.material = mat;
                r.material.color = color;
                
            }
            highlightObject.SetActive(false);
        }
        else if(changeColor)
        {
            renderers = target.GetComponentsInChildren<Renderer>();
            lastColor = renderers[0].material.color;
        }else if (changeUI)
        {
            graphics = target.GetComponentsInChildren<Image>();
            lastColor = graphics[0].color;
        }



    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (auto)
        {
            if (timer > starttime&&!isOn)
            {
                Highlight();
            }
            if (timer > starttime+ duration && isOn)
            {
                Off();
                timer = 0;
                enabled = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (isOn)
                {
                    Off();
                }
                else
                {
                    Highlight();
                }
            }

        }

    }

    public void Highlight()
    {
        isOn = true;
        if (changeshader)
        {
            highlightObject.SetActive(true);
        }
        else if(changeColor)
        {
            foreach(var r in renderers)
            {
                r.material.color = color;
            }
        }
        else if (changeUI)
        {
            foreach (var r in graphics)
            {
                r.color = color;
            }
        }
    }

    public void Off()
    {
        isOn = false;
        if (changeshader)
        {
            highlightObject.SetActive(false);
        }
        else if(changeColor)
        {
            foreach (var r in renderers)
            {
                r.material.color = lastColor;
            }
        }
        else if (changeUI)
        {
            foreach (var r in graphics)
            {
                r.color = lastColor;
            }
        }
    }
}
