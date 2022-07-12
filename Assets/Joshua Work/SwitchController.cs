using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchController : MonoBehaviour
{
    public ParticleSystem spiralSystem;
    public ParticleSystem wormholeSystem;
    public float timeToTeleport;
    public string nextScene;
    public float sceneLoad;
    public Canvas canvas;

    private float time;
    private bool ready;
    private TransitionController transitionController;
    
    void Start()
    {
        time = 0f;
        ready = false;
        transitionController = canvas.GetComponent<TransitionController>();
        spiralSystem.Play();
    }

    void Update()
    {
        if (ready)
        {
            time += Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        ready = true;
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log(time);
        if (time > timeToTeleport)
        {
            time = 0f;
            ready = false;
            StartCoroutine(PlayWormhole());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited");
        time = 0f;
        ready = false;
    }
    IEnumerator PlayWormhole()
    {
        wormholeSystem.Play();
        transitionController.fadeOut = true;
        yield return new WaitForSeconds(sceneLoad);
        Debug.Log("testing");
        SceneManager.LoadScene(nextScene);
    }
}
