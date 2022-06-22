using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;


//Name the scenes Test00-Test09, use the number keys on top of the alphabet keyboard to load them
//0 loads Test00, 1 loads Test01, and so on.

public class sceneLoading : MonoBehaviour
{
    public IMixedRealitySceneSystem sceneSystem;
    public int currentScene = 0;


    // Start is called before the first frame update
    void Awake()
    {
        //Initialize scene system
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
    }

    private async void Scene_0()
    {
        //Unload previous scene
        await sceneSystem.UnloadContent("Test0" + currentScene);

        //Load scene 0
        await sceneSystem.LoadContent("Test00");
        currentScene = 0;
    }

    private async void Scene_1()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test01");
        currentScene = 1;
    }

    private async void Scene_2()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test02");
        currentScene = 2;
    }

    private async void Scene_3()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test03");
        currentScene = 3;
    }

    private async void Scene_4()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test04");
        currentScene = 4;
    }

    private async void Scene_5()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test05");
        currentScene = 4;
    }

    private async void Scene_6()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test06");
        currentScene = 4;
    }

    private async void Scene_7()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test07");
        currentScene = 4;
    }

    private async void Scene_8()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test08");
        currentScene = 4;
    }

    private async void Scene_9()
    {
        await sceneSystem.UnloadContent("Test0" + currentScene);
        await sceneSystem.LoadContent("Test09");
        currentScene = 4;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Scene_0();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Scene_1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Scene_2();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Scene_3();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Scene_4();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Scene_5();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Scene_6();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Scene_7();
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Scene_8();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Scene_9();
        }
    }
}
