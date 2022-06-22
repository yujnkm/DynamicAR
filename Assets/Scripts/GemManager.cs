using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
public enum Orientation
{
    Horizontal,
    Vertical,
    None
}

public enum Texture
{
    Smooth,
    Rough,
    None
}

public enum Hiding
{
    Physical00,
    Physical01,
    None
}

[System.Serializable]
public class GemDetails
{
    public string name;
    public Orientation orientation;
    public Texture texture;
    public Hiding hiding;
    public Vector3 position;

    public GemDetails(string s, Hiding h, Vector3 v)
    {
        name = s;
        orientation = Orientation.None;
        texture = Texture.None;
        hiding = h;
        position = v;

    }

    public GemDetails(string s, Orientation o, Texture t, Hiding h, Vector3 v)
    {
        name = s;
        orientation = o;
        texture = t;
        hiding = h;
        position = v;

    }
}

[System.Serializable]
public class GemList
{
    public List<GemDetails> gems;
}

[System.Serializable]
public class Gem
{
    public int[] trial0;
    public int[] trial1;
    public int[] trial2;
    public int[] trial3;
    public int[] trial4;
    public int[] trial5;
    public int[] trial6;
    public int[] trial7;

    public List<int[]> trials;

    public void GemToList()
    {
        trials = new List<int[]>();
        trials.Add(trial0);
        trials.Add(trial1);
        trials.Add(trial2);
        trials.Add(trial3);
        trials.Add(trial4);
        trials.Add(trial5);
        trials.Add(trial6);
        trials.Add(trial7);

    }

}

[System.Serializable]
public class Root
{
    public List<Gem> gems;
}

public struct GemComb
{
    public Orientation o;
    public Texture t;

    public GemComb(Orientation or, Texture te)
    {
        o = or;
        t = te;
    }
}

public class GemManager : MonoBehaviour
{
    public Root pGem;
    public Root vGem;
    public Root nGem;
    public GemList p0Loc;
    public GemList p1Loc;
    public GemList nLoc;
    public Material rough;
    public Material smooth;
    public Dictionary<string, GemComb> ot;

    public GameObject[] prefabs;
    public GameObject AllGemLocations;
    
    public string id;
    public IEnumerator Upload(string type, string requestBodyString)
    {
        //https://github.com/Kubos-cz/Unity-WebRequest-Example


        // Create request URL string based on type of data to upload
        string url = Const.PROJECT_PATH + "/" + id + "/gemLocations/" + type + ".json";
        if (type.Contains("Scene"))
        {
            url = Const.PROJECT_PATH + "/" + id + "/gemTrialLocations/" + type + ".json";
        }

        // Convert Json body string into a byte array
        byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
        // Create new UnityWebRequest, pass on our url and body as a byte array
        UnityWebRequest webRequest = UnityWebRequest.Put(url, requestBodyData);
        // Specify that our method is of type 'patch'
        webRequest.method = "PATCH";

        // Send the request itself
        yield return webRequest.SendWebRequest();
        //Debug.Log("Sent Request");
        // Check for errors
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            // Invoke error action
            Debug.Log("Error!");
            //Debug.Log(webRequest.isNetworkError);
            //Debug.Log(webRequest.isHttpError);
            Debug.Log(webRequest.error);
            //onDeleteRequestError?.Invoke(webRequest.error);
        }
        else
        {
            // Check when response is received
            if (webRequest.isDone)
            {
                // Invoke success action
                //Debug.Log("Success! " + data);
                //onDeleteRequestSuccess?.Invoke("Patch Request Completed");
            }
        }
    }
    public void SaveLocations(int physical)
    {
        AllGemLocations.SetActive(true);
        GameObject gems = GameObject.Find("Physical00Gems");
        print("P00");
        print(gems.transform.childCount);
        p0Loc.gems = new List<GemDetails>();
        for (int i = 0; i < gems.transform.childCount; i++)
        {
            p0Loc.gems.Add(new GemDetails("Gem" + i.ToString("D2") + "P0", Hiding.Physical00, gems.transform.GetChild(i).transform.position));
        }
        

        gems = GameObject.Find("Physical01Gems");
        p1Loc.gems = new List<GemDetails>();
        for (int i = 0; i < gems.transform.childCount; i++)
        {
            p1Loc.gems.Add(new GemDetails("Gem" + (i + gems.transform.childCount).ToString("D2") + "P1", Hiding.Physical01, gems.transform.GetChild(i).transform.position));
        }

        print("P00");
        print(gems.transform.childCount);
        if (physical == 0)
        {
            gems = GameObject.Find("PhysicalGems");
            for (int i = 0; i < gems.transform.childCount; i++)
            {
                p0Loc.gems.Add(new GemDetails("Gem" + (p0Loc.gems.Count + gems.transform.childCount).ToString("D2") + "P0", Hiding.Physical00, gems.transform.GetChild(i).transform.position));
            }

            gems = GameObject.Find("VirtualGems");
            for (int i = 0; i < gems.transform.childCount; i++)
            {
                p1Loc.gems.Add(new GemDetails("Gem" + (p1Loc.gems.Count + gems.transform.childCount).ToString("D2") + "P1", Hiding.Physical01, gems.transform.GetChild(i).transform.position));
            }
        }
        else
        {
            gems = GameObject.Find("VirtualGems");
            for (int i = 0; i < gems.transform.childCount; i++)
            {
                p0Loc.gems.Add(new GemDetails("Gem" + (p0Loc.gems.Count + gems.transform.childCount).ToString("D2") + "P0", Hiding.Physical00, gems.transform.GetChild(i).transform.position));
            }

            gems = GameObject.Find("PhysicalGems");
            for (int i = 0; i < gems.transform.childCount; i++)
            {
                p1Loc.gems.Add(new GemDetails("Gem" + (p1Loc.gems.Count + gems.transform.childCount).ToString("D2") + "P1", Hiding.Physical01, gems.transform.GetChild(i).transform.position));
            }
        }

        StartCoroutine(Upload("physical00", JsonUtility.ToJson(p0Loc)));
        StartCoroutine(Upload("physical01", JsonUtility.ToJson(p1Loc)));


        gems = GameObject.Find("NoneGems");
        nLoc.gems = new List<GemDetails>();
        for (int i = 0; i < gems.transform.childCount; i++)
        {
            nLoc.gems.Add(new GemDetails("Gem" + (i + gems.transform.childCount).ToString("D2") + "V", Hiding.None, gems.transform.GetChild(i).transform.position));
        }

        print("None");
        print(gems.transform.childCount);
        StartCoroutine(Upload("none", JsonUtility.ToJson(nLoc)));
        AllGemLocations.SetActive(false);
    }

    public void ReadCombinations()
    {
        string json = "";
        using (StreamReader myFile = new StreamReader(Path.Combine(Application.streamingAssetsPath, "physical00_gem_combinations.json")))
        {
            json = myFile.ReadToEnd();
        }

        pGem = JsonUtility.FromJson<Root>(json);
        foreach (Gem g in pGem.gems)
        {
            g.GemToList();
        }
        using (StreamReader myFile = new StreamReader(Path.Combine(Application.streamingAssetsPath, "physical01_gem_combinations.json")))
        {
            json = myFile.ReadToEnd();
        }

        vGem = JsonUtility.FromJson<Root>(json);
        foreach (Gem g in vGem.gems)
        {
            g.GemToList();
        }
        using (StreamReader myFile = new StreamReader(Path.Combine(Application.streamingAssetsPath, "none_gem_combinations.json")))
        {
            json = myFile.ReadToEnd();
        }

        nGem = JsonUtility.FromJson<Root>(json);
        foreach (Gem g in nGem.gems)
        {
            g.GemToList();
        }

    }

    public void CreateGemLayouts()
    {
        for (int i = 0; i < pGem.gems.Count; i++)
        //for (int i = 0; i < 1; i++)
        {
            GameObject current = new GameObject();
            current.transform.parent = gameObject.transform;
            current.name = "Option" + i.ToString("D2");
            for (int j = 0; j < Const.NUM_LAYOUTS; j++)
            {
                GameObject trial = new GameObject();
                trial.transform.parent = current.transform;
                trial.name = "Scene" + j.ToString("D2");
                print(trial.name);
                int num = 0;
                GemList trialGems = new GemList();
                trialGems.gems = new List<GemDetails>();
                for (int k = 0; k < pGem.gems[i].trials[0].Length; k++)
                {
                    int gemIndex = pGem.gems[i].trials[j][k];
                    GameObject myGem = Instantiate(prefabs[num], p0Loc.gems[gemIndex].position, new Quaternion(0, 0, 0, 0), trial.transform);
                    myGem.name = "Gem" + ot[num.ToString()].o + p0Loc.gems[gemIndex].hiding + ot[num.ToString()].t + gemIndex + prefabs[num].name;
                    trialGems.gems.Add(new GemDetails(myGem.name, ot[num.ToString()].o, ot[num.ToString()].t, p0Loc.gems[gemIndex].hiding, p0Loc.gems[gemIndex].position));
                    if (ot[num.ToString()].t == Texture.Rough)
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = rough;
                    }
                    else
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = smooth;
                    }
                    num = (num + 1) % 4;
                }

                for (int k = 0; k < vGem.gems[i].trials[0].Length; k++)
                {
                    int gemIndex = vGem.gems[i].trials[j][k];
                    GameObject myGem = Instantiate(prefabs[num], p1Loc.gems[gemIndex].position, new Quaternion(0, 0, 0, 0), trial.transform);
                    myGem.name = "Gem" + ot[num.ToString()].o + p1Loc.gems[gemIndex].hiding + ot[num.ToString()].t + gemIndex + prefabs[num].name;
                    trialGems.gems.Add(new GemDetails(myGem.name, ot[num.ToString()].o, ot[num.ToString()].t, p1Loc.gems[gemIndex].hiding, p1Loc.gems[gemIndex].position));
                    if (num > 1)
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = rough;
                    }
                    else
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = smooth;
                    }
                    num = (num + 1) % 4;
                }
                print(nGem.gems[i].trials[0].Length);
                for (int k = 0; k < nGem.gems[i].trials[0].Length; k++)
                {
                    int gemIndex = nGem.gems[i].trials[j][k];
                    GameObject myGem = Instantiate(prefabs[num], nLoc.gems[gemIndex].position, new Quaternion(0, 0, 0, 0), trial.transform);
                    myGem.name = "Gem" + ot[num.ToString()].o + nLoc.gems[gemIndex].hiding + ot[num.ToString()].t + gemIndex + prefabs[num].name;
                    trialGems.gems.Add(new GemDetails(myGem.name, ot[num.ToString()].o, ot[num.ToString()].t, nLoc.gems[gemIndex].hiding, nLoc.gems[gemIndex].position));
                    if (num > 1)
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = rough;
                    }
                    else
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = smooth;
                    }
                    num = (num + 1) % 4;
                }

                StartCoroutine(Upload(trial.name, JsonUtility.ToJson(trialGems)));

            }
            /*            // Set the path as within the Assets folder,
                        // and name it as the GameObject's name with the .Prefab format
                        string localPath = "Assets/" + current.name + ".prefab";

                        // Make sure the file name is unique, in case an existing Prefab has the same name.
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                        // Create the new Prefab.
                        PrefabUtility.SaveAsPrefabAssetAndConnect(current, localPath, InteractionMode.UserAction);
                        print(localPath);*/
        }


    }

    public void LoadGemLayouts()
    {
        //for (int i = 0; i < pGem.gems.Count; i++)
        for (int i = 0; i < 1; i++)
        {
            GameObject current = GameObject.Find("Gems");
            /*            current.transform.parent = gameObject.transform;
                        current.name = "Option" + i.ToString("D2");*/
            for (int j = 0; j < Const.NUM_LAYOUTS; j++)
            {
                GameObject trial = new GameObject();
                trial.transform.parent = current.transform;
                trial.name = "Scene" + j.ToString("D2");

                int num = 0;
                GemList trialGems = new GemList();
                trialGems.gems = new List<GemDetails>();
                for (int k = 0; k < pGem.gems[i].trials[0].Length; k++)
                {
                    int gemIndex = pGem.gems[i].trials[j][k];
                    GameObject myGem = Instantiate(prefabs[num], p0Loc.gems[gemIndex].position, new Quaternion(0, 0, 0, 0), trial.transform);
                    myGem.name = "Gem" + ot[num.ToString()].o + p0Loc.gems[gemIndex].hiding + ot[num.ToString()].t + gemIndex.ToString("D2");
                    trialGems.gems.Add(new GemDetails(myGem.name, ot[num.ToString()].o, ot[num.ToString()].t, p0Loc.gems[gemIndex].hiding, p0Loc.gems[gemIndex].position));
                    if (ot[num.ToString()].t == Texture.Rough)
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = rough;
                    }
                    else
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = smooth;
                    }
                    num = (num + 1) % 4;
                }
                print("a");
                for (int k = 0; k < vGem.gems[i].trials[0].Length; k++)
                {
                    int gemIndex = vGem.gems[i].trials[j][k];
                    GameObject myGem = Instantiate(prefabs[num], p1Loc.gems[gemIndex].position, new Quaternion(0, 0, 0, 0), trial.transform);
                    myGem.name = "Gem" + ot[num.ToString()].o + p1Loc.gems[gemIndex].hiding + ot[num.ToString()].t + gemIndex.ToString("D2");
                    trialGems.gems.Add(new GemDetails(myGem.name, ot[num.ToString()].o, ot[num.ToString()].t, p1Loc.gems[gemIndex].hiding, p1Loc.gems[gemIndex].position));
                    if (num > 1)
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = rough;
                    }
                    else
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = smooth;
                    }
                    num = (num + 1) % 4;
                }
                print("b");
                for (int k = 0; k < nGem.gems[i].trials[0].Length; k++)
                {
                    print(k);
                    int gemIndex = nGem.gems[i].trials[j][k];
                    GameObject myGem = Instantiate(prefabs[num], nLoc.gems[gemIndex].position, new Quaternion(0, 0, 0, 0), trial.transform);
                    myGem.name = "Gem" + ot[num.ToString()].o + nLoc.gems[gemIndex].hiding + ot[num.ToString()].t + gemIndex.ToString("D2");
                    trialGems.gems.Add(new GemDetails(myGem.name, ot[num.ToString()].o, ot[num.ToString()].t, nLoc.gems[gemIndex].hiding, nLoc.gems[gemIndex].position));
                    if (num > 1)
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = rough;
                    }
                    else
                    {
                        myGem.GetComponent<MeshRenderer>().materials[0] = smooth;
                    }
                    num = (num + 1) % 4;
                }
                print("c");
                StartCoroutine(Upload(trial.name, JsonUtility.ToJson(trialGems)));

            }
            /*            // Set the path as within the Assets folder,
                        // and name it as the GameObject's name with the .Prefab format
                        string localPath = "Assets/" + current.name + ".prefab";

                        // Make sure the file name is unique, in case an existing Prefab has the same name.
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                        // Create the new Prefab.
                        PrefabUtility.SaveAsPrefabAssetAndConnect(current, localPath, InteractionMode.UserAction);
                        print(localPath);*/
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        //AllGemLocations = GameObject.Find("AllGemLocations");
    }

    public void InitializeGems(string userID, int physical)
    {
        id = userID;
        ot = new Dictionary<string, GemComb>();
        ot["0"] = new GemComb(Orientation.Horizontal, Texture.Smooth);
        ot["1"] = new GemComb(Orientation.Vertical, Texture.Smooth);
        ot["2"] = new GemComb(Orientation.Horizontal, Texture.Rough);
        ot["3"] = new GemComb(Orientation.Vertical, Texture.Rough);
        SaveLocations(physical);
        ReadCombinations();
        LoadGemLayouts();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
