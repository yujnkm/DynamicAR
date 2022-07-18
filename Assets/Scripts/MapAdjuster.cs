using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAdjuster : MonoBehaviour
{

    public static MapAdjuster instance;

    [Header("Manual Adjustment")]
    [SerializeField] GameObject VRPlayer;
    
    [Tooltip("Set up the value for each manual adjustment. Unit: 1f = 1 Meter")]
    public float YoffsetStep = 0.05f;
    
    [Tooltip("Keyboard key to move eye height up for one step.")]
    [SerializeField] KeyCode jKey = KeyCode.J;
    
    [Tooltip("Keyboard key to move eye height down for one step.")]
    [SerializeField] KeyCode mKey = KeyCode.M;
    
    [Tooltip("Keyboard key to turn right.")]
    [SerializeField] KeyCode nKey = KeyCode.N;
    
    [Tooltip("Keyboard key to turn left.")]
    [SerializeField] KeyCode hKey = KeyCode.H;
    
    [Tooltip("Keyboard key to scale up.")]
    [SerializeField] KeyCode kKey = KeyCode.K;
    
    [Tooltip("Keyboard key to scale down.")]
    [SerializeField] KeyCode lKey = KeyCode.L;
    
    [Tooltip("Keyboard key to move forwards.")]
    [SerializeField] KeyCode wKey = KeyCode.W;
    
    [Tooltip("Keyboard key to move backwards.")]
    [SerializeField] KeyCode sKey = KeyCode.S;
    
    [Tooltip("Keyboard key to move left.")]
    [SerializeField] KeyCode aKey = KeyCode.A;
    
    [Tooltip("Keyboard key to move right.")]
    [SerializeField] KeyCode dKey = KeyCode.D;

    [Header("Auto Adjustment")]
    
    [Tooltip("If true, an automatic adjustment of eye height will be applied at the very beginning")]
    [SerializeField] bool AutoAdjust = true;
    
    [Tooltip("The height offset to be automatically applied when AutoAdjust is on.")]
    [SerializeField] float AutoHeightOffset = -0.9f;

    private Transform _vrTransform;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    void Start()
    {
        // Cache transform to reduce extern calls
        _vrTransform = VRPlayer.transform;
        if (AutoAdjust) _vrTransform.Translate(new Vector3(0, AutoHeightOffset, 0));
    }
    void Update()
    {
        if (VRPlayer.activeSelf)
        {
            if (Input.GetKeyDown(jKey)) _vrTransform.Translate(new Vector3(0, YoffsetStep, 0));
            if (Input.GetKeyDown(mKey)) _vrTransform.Translate(new Vector3(0, -YoffsetStep, 0));

            if (Input.GetKeyDown(wKey)) _vrTransform.Translate(new Vector3(-0.1f, 0, 0));
            if (Input.GetKeyDown(sKey)) _vrTransform.Translate(new Vector3(0.1f, 0, 0));

            if (Input.GetKeyDown(aKey)) _vrTransform.Translate(new Vector3(0, 0, -0.1f));
            if (Input.GetKeyDown(dKey)) _vrTransform.Translate(new Vector3(0, 0, 0.1f));

            if (Input.GetKeyDown(nKey)) _vrTransform.Rotate(Vector3.up, 1);
            if (Input.GetKeyDown(hKey)) _vrTransform.Rotate(Vector3.down, 1);

            if (Input.GetKeyDown(kKey)) _vrTransform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            if (Input.GetKeyDown(lKey)) _vrTransform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
    public void HeadsetUp()
    {
        if (VRPlayer.activeSelf)
        {
            _vrTransform.Translate(new Vector3(0, YoffsetStep, 0));
        }
    }
    public void HeadsetDown()
    {

        if (VRPlayer.activeSelf)
        {
            _vrTransform.Translate(new Vector3(0, -YoffsetStep, 0));
        }

    }
}
