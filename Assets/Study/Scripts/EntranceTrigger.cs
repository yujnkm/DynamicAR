using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EntranceTrigger : MonoBehaviour
{
    public UnityEvent enterEvent;
    public Transform target;
    public Vector3 destination;
    public float distance;
    private bool isInside;
    private bool lastIsInside;
    public float coolDownTime = 5;
    private float t;
    private Color red;
    private Color green;
    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
        red = new Color(1.0f, 0, 0, 0.3f);
        green = new Color(0.0f, 1.0f, 0, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if((target.position - (transform.TransformPoint(destination))).magnitude < distance)
        {
            isInside = true;
        }
        else
        {
            isInside = false;
        }

        if (lastIsInside!= isInside)
        {
            if (isInside)
            {
                OnEnter();
            }
            else
            {
                OnExit();
            }
        }

        lastIsInside = isInside;
    }


    public void OnEnter()
    {
        if (t > coolDownTime)
        {
            enterEvent.Invoke();
        }
        t = 0.0f;
    }

    public void OnExit()
    {

        t = 0.0f;
    }



    private void OnDrawGizmos()
    {
        if (!isInside)
        {
            Gizmos.color = red;
        }
        else
        {
            Gizmos.color = green;
        }

        Gizmos.DrawSphere(transform.TransformPoint(destination), distance);
    }

   
}
