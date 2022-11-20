using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ArrowController : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject arrowLocation;
    public float rotateSpeed;
    public float amplitude;
    public float frequency;
    public float fadeTime;

    private bool fadeIn;
    private bool fadeOut;
    private Renderer _renderer;

    #region Singleton
    public static ArrowController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    void OnEnable()
    {
        fadeIn = false;
        fadeOut = false;
    }
    void Start()
    {
        _renderer = transform.GetChild(0).GetComponent<Renderer>();
        Color color = _renderer.material.color;
        color.a = 0f;
        _renderer.material.color = color;
    }
    void Update()
    {
        transform.position = new Vector3(arrowLocation.transform.position.x, transform.position.y, arrowLocation.transform.position.z);

        //Finds position of the target and rotate towards it
        Vector3 targetPosition = new Vector3(targetObject.transform.position.x, transform.position.y, targetObject.transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed);

        //Arrow oscillates back and forth (just a visual effect)
        Vector3 position = transform.position;
        position += transform.forward * Mathf.Sin(frequency * Time.time) * amplitude;
        transform.position = position;

        //fades transitions
        if (fadeIn)
        {
            Color color = _renderer.material.color;
            color.a = color.a + 1f / fadeTime * Time.deltaTime;
            if (color.a > 1)
            {
                color.a = 1;
                fadeIn = false;
            }
            _renderer.material.color = color;
        }
        if (fadeOut)
        {
            Color color = _renderer.material.color;
            color.a = color.a - 1f / fadeTime * Time.deltaTime;
            if (color.a < 0)
            {
                color.a = 0;
                fadeOut = false;
            }
            _renderer.material.color = color;
        }
    }
    public void ArrowFadeIn()
    {
        fadeIn = true;
    }
    public void ArrowFadeOut()
    {
        fadeOut = true;
    }
}
