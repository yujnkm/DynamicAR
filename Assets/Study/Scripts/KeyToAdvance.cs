using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyToAdvance : MonoBehaviour
{
    public KeyCode key;
    public float timer;
    public int jumpScene;

    private float t;
    private TutorialClip tutorialClip;

    void Start()
    {
        tutorialClip = GetComponent<TutorialClip>();
    }

    void Update()
    {
        t += Time.deltaTime;

        if (t > timer)
        {
            if (Input.GetKeyDown(key))
            {
                if (key == KeyCode.P || key == KeyCode.N)
                {
                    tutorialClip.Done();
                }
                else if (key == KeyCode.A)
                {
                    tutorialClip.JumpTo();
                }
                else if (key == KeyCode.B)
                {
                    tutorialClip.MoveBack();
                }
            }
        }
    }
}
