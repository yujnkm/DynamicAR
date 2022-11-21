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
    public float timeToTeleport;
    public string nextScene;
    public float sceneLoad;

    private float time;
    private bool ready;

    #region Singleton
    public static SwitchController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    void OnEnable()
    {
        time = 0f;
        ready = false;
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
        if (other.gameObject.tag == "MainCamera")
        {
            if (time > timeToTeleport)
            {
                time = 0f;
                ready = false;
                TutorialController.Instance.Next();
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
        if (other.gameObject.tag == "MainCamera")
        {
            time = 0f;
            ready = false;
        }
    }
}
