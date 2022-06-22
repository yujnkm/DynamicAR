using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomToggleObjects : MonoBehaviour
{
    public GameObject physical;
    internal GameObject physicalOcclusion;
    public GameObject environment;
    public Material occlusionMat;

    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        CreateOccluders();
    }

    public void CreateOccluders()
    {
        physicalOcclusion = GameObject.Instantiate(physical, physical.transform.position, physical.transform.rotation, physical.transform.parent);
        physicalOcclusion.name = "physicalOcclusion";
        var renderers = physicalOcclusion.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.material = occlusionMat;
        }
        physicalOcclusion.SetActive(!physical.activeSelf);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void TogglePhysical()
    {
        if (physical)
        {
            physical.SetActive(!physical.activeSelf);
            physicalOcclusion.SetActive(!physical.activeSelf);
        }

    }


    public void ToggleMenu()
    {
        if (menu)
        {
            menu.SetActive(!menu.activeSelf);
        }

    }

    public void ToggleEnv()
    {
        if (environment)
        {
            environment.SetActive(!environment.activeSelf);
        }

    }

    public void ExitApplication()
    {
        Application.Quit();

    }
}
