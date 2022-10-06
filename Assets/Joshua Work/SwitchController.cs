using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Attached to the spiral system,
 * Will "teleport" user to the next scene
 */
public class SwitchController : MonoBehaviour
{
    public ParticleSystem spiralSystem;
    public ParticleSystem wormholeSystem;
    public float timeToTeleport;
    public string nextScene;
    public float sceneLoad;
    //public Canvas canvas;

    private float time;
    private bool ready;
    //private TransitionController transitionController;
    
    void Start()
    {
        time = 0f;
        ready = false;
        //transitionController = canvas.GetComponent<TransitionController>();
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
        //Checks if it is the player that has entered the box collider around the spiral
        if (other.gameObject.tag == "MainCamera")
        {
            Debug.Log("Entered");
            ready = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        /*
         * If the user stays within the spiral for a few seconds,
         * then that means the user wants to switch to the next scene.
         * Thus, the next key from the tutorial script will automatically be played
         * to switch to the next scene.
         */
        //Debug.Log(time);
        if (other.gameObject.tag == "MainCamera")
        {
            if (time > timeToTeleport)
            {
                time = 0f;
                ready = false;
                GameObject.FindObjectOfType<TutorialController>().Next();
                //StartCoroutine(PlayWormhole());
            }
        }
    }
    /*
     * If the user leaves the spiral before the wait time,
     * then the time will reset and the scene will not switch,
     * as we are assuming that it was an accident the user
     * entered the box collider around the spiral.
     */
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited");
        if (other.gameObject.tag == "MainCamera")
        {
            time = 0f;
            ready = false;
        }
    }
/*    IEnumerator PlayWormhole()
    {
        wormholeSystem.Play();
        transitionController.fadeOut = true;
        yield return new WaitForSeconds(sceneLoad);
        Debug.Log("testing");
        SceneManager.LoadScene(nextScene);
    }*/
}
