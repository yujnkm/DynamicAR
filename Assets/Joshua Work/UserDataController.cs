using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserDataController : MonoBehaviour
{
    public GameObject startLocation;

    private Vector3 position;
    private Quaternion rotation;
    private Camera camera;
    private string curSceneName;
    private Scene scene;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        camera = Camera.main;
        scene = SceneManager.GetActiveScene();
        curSceneName = scene.name;
        camera.transform.position = startLocation.transform.position;
    }

    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != curSceneName)
        {
            curSceneName = scene.name;
            camera = Camera.main;
            camera.transform.position = position;
            camera.transform.rotation = rotation;
        }
        position = Camera.main.transform.position;
        rotation = Camera.main.transform.rotation;
    }
}
