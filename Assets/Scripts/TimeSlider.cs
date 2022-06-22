using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeSlider : MonoBehaviour
{
    public Slider slider;
    public AnimationCurve curve;
    public Text text;
    public float initvalue;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = initvalue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTimeScale(float ts)
    { 
        Time.timeScale = curve.Evaluate(ts);
        text.text = Time.timeScale.ToString("#.#")+"x";
    }
}
