using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

[System.Serializable]
public class Session2
{
    public int call_word;
    public int word_track;
    public int delay_track;
    public int map;
    public bool target_present;
    public int target_combination;
    public bool audio_present;
}


[System.Serializable]
public class Trial{
    public string userid;
    public int index;
    public Session2[] sessions;
}

[System.Serializable]
public struct AllTrials
{
    public Trial[] trials;
}


public class StudyParser : MonoBehaviour
{
    public string jsonpath;
    public AllTrials alltrials;
    public UnityEvent OnStudyLoaded;
    // Start is called before the first frame update
    void Start()
    {
       string jsonstring = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, jsonpath));
        alltrials = JsonUtility.FromJson<AllTrials>(jsonstring);
        OnStudyLoaded.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


