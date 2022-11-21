using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObjectController : MonoBehaviour
{
    public GameObject player;
    public GameObject[] targetObjectList;

    [Tooltip("Leave as \"none\" if targetObject has collider")]
    public GameObject[] actualObjectList;

    [Tooltip("Set a negative number for objects that already have a detection system (such as play points)")]
    public float[] targetDistanceList;

    private bool[] objectFound;
    private int numObjects;
    private int curObject = 0;

    #region Singleton
    public static TargetObjectController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    private void OnEnable()
    {
        curObject = 0;
        numObjects = targetObjectList.Length;
        objectFound = new bool[numObjects];

        /*
         * sets the initial targets
         * it is important that clip 0 does not have a gameobject with this script attached
         * otherwise, it is not guaranted that ArrowController and IndicatorSystemController have been enabled
         */
        ArrowController.Instance.targetObject = targetObjectList[curObject];
        IndicatorSystemController.Instance.targetObject = targetObjectList[curObject];
        IndicatorSystemController.Instance.actualObject = actualObjectList[curObject];
    }

    void Update()
    {
        //if targetDistancList[curObject] is negative, other mechanism will be used to check for player detection
        if (targetDistanceList[curObject] > 0 && 
            Vector3.Distance(targetObjectList[curObject].transform.position, player.transform.position) 
            < targetDistanceList[curObject])
        {
            ObjectReached(curObject);
        }

        //if current object has been found, target will be set to the next target in list
        if (curObject < numObjects && objectFound[curObject])
        {
            //makes sure index does not go out of bounds
            curObject = Mathf.Min(curObject + 1, numObjects - 1);

            ArrowController.Instance.targetObject = targetObjectList[curObject];
            IndicatorSystemController.Instance.targetObject = targetObjectList[curObject];
            IndicatorSystemController.Instance.actualObject = actualObjectList[curObject];
        }
    }
    public void FindAndMarkObject(GameObject gameObject)
    {
        for (int i = 0; i < numObjects; i++)
        {
            if (targetObjectList[i] == gameObject)
            {
                ObjectReached(i);
                return;
            }
        }
    }
    private void ObjectReached(int index)
    {
        objectFound[index] = true;
    }
}
