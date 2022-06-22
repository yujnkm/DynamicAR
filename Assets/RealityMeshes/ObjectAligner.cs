using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ObjectAligner : MonoBehaviour
{

    public Transform point1;
    public Transform point2;
    public Transform point1w;
    public Transform point2w;

    public bool align;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValidate()
    {
        if (align)
        {
            align = false;
            Align();
        }
    }

    public  void Align()
    {

        float scale = (point1w.position - point2w.position).magnitude / (point1.position - point2.position).magnitude;
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(scale, scale, scale));


        //Vector3 from = point2.position - point1.position;
        //Vector3 to = point2w.position - point1w.position;

        //transform.rotation = Quaternion.FromToRotation(from, to);

        transform.position = transform.position+(point1w.position - point1.position);


        point1w.LookAt(point2);

        transform.parent = point1w;

        point1w.LookAt(point2w);

        transform.parent = null;


        //transform.rotation = Quaternion.LookRotation(point2w.position - point2.position, point1.up);
        //point1.localScale = point1w.localScale;
        //point2.localScale = point2w.localScale;
    }
}
