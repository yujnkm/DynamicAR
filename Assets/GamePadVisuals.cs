using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePadVisuals : MonoBehaviour
{
    public Renderer A;
    public Renderer X;
    public Renderer Y;
    public Renderer B;
    public AudioClip pop;
    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()

    {
        if (Input.anyKeyDown)
        {
            source.PlayOneShot(pop);
        }
        /*
        X->h
        A->g
        B->j
        Y->i
        */


        if (Input.GetKeyDown(KeyCode.H))
        {
            X.material.color = Color.red;
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            X.material.color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            A.material.color = Color.red;
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            A.material.color = Color.white;
        }


        if (Input.GetKeyDown(KeyCode.J))
        {
            B.material.color = Color.red;
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            B.material.color = Color.white;
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            Y.material.color = Color.red;
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            Y.material.color = Color.white;
        }



    }
}
