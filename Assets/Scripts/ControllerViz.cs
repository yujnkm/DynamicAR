using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControllerViz : MonoBehaviour
{
    public List<Image> buttons;

    public Color released;
    public Color pressed;
    public AudioSource source;
    public AudioClip clip;

    public IEnumerator OnButtonPressed(int id)
    {
        buttons[id].color = pressed;
        yield return new WaitForSeconds(0.3f);
        buttons[id].color = released;
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void ButtonPressed(int id)
    {
        Debug.Log("Button pressed " + id);
        source.PlayOneShot(clip);
        StartCoroutine(OnButtonPressed(id));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
