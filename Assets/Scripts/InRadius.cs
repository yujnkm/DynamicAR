using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//Gem : collider (isTrigger)
//Camera: script, rigidbody (isKinematic), collider (isTrigger)

public enum Status
{
    New,
    InProgress,
    Log
}
[System.Serializable]
public struct ProximityLog
{
    public string objectName;
    public Status status;
    public long startTime;
    public long length;
}
public class InRadius : MonoBehaviour
{

    public Dictionary<string, ProximityLog> gem = new Dictionary<string, ProximityLog>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnTriggerEnter(Collider col)
    {
        //print("enter");
        if(col.gameObject.tag.Equals("gem"))
        {
            ProximityLog temp = new ProximityLog();
            temp.objectName = col.gameObject.name;
            temp.status = Status.InProgress;
            temp.startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            gem[col.gameObject.name] = temp;
        }
        

    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag.Equals("gem"))
        {
            //print("exit");
            ProximityLog temp = gem[col.gameObject.name];
            temp.status = Status.Log;
            temp.length = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - temp.startTime;
            gem[col.gameObject.name] = temp;
            //print(gem.Keys.ToList());
        }
    }
}

