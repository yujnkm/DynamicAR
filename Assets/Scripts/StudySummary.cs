using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class SessionSummary
{
    public string userID;
    public int sessionIndex;
    public int physical;
    public bool audio;
    public int callWord;
    public int audioTrack;
    public string map;

    public float physicalAccuracy;
    public float virtualAccuracy;
    public float floatingAccuracy;
    public float distanceTraveled;
    public float totalRotation;
    public float totalSessionTime;
    public float audioAccuracy;

    public int totalAudioCalls;
    public int correctAudioClicks;

    public int visitedPhisycalGems;
    public int visitedVirtualGems;
    public int visitedFloatingGems;


    public int wrongPhysicalGems;
    public int wrongVirtualGems;
    public int wrongFloatingGems;


    public int repeatedPhysicalGems;
    public int repeatedVirtualGems;
    public int repeatedFloatingGems;

    public SessionSummary(float physicalAccuracy, float virtualAccuracy, float floatingAccuracy)
    {
        this.physicalAccuracy = physicalAccuracy;
        this.virtualAccuracy = virtualAccuracy;
        this.floatingAccuracy = floatingAccuracy;
    }

    public SessionSummary(int physical, bool audio, int callWord, int audioTrack, string map)
    {
        this.physical = physical;
        this.audio = audio;
        this.callWord = callWord;
        this.audioTrack = audioTrack;
        this.map = map;
    }

    public SessionSummary(Session session)
    {
        this.physical = session.physical;
        this.audio = session.audio;
        this.callWord = session.callWord;
        this.audioTrack = session.audioTrack;
        this.map = session.map;
    }
}
public class StudySummary : MonoBehaviour
{
    public List<SessionSummary> sessionSummaries;
    // Start is called before the first frame update
    void Start()
    {
           //sessionSummaries = new List<SessionSummary>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFromDescription(StudyDescription description)
    {
        sessionSummaries = new List<SessionSummary>();
        for(int i = 0; i < description.studySessions.sessions.Count; i++)
        {
            var sessionSummary = new SessionSummary(description.studySessions.sessions[i]);
            sessionSummary.userID = description.userID;
            sessionSummary.sessionIndex = i;
            sessionSummaries.Add(sessionSummary);
        }
    }

    public void SaveToJSON(string folderpath,string filepath)
    {
        string directory = folderpath + "/extract/";
        string path = directory + filepath + "_ex.json";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonUtility.ToJson(this);
        if (!File.Exists(path))
        {
            var stream = File.CreateText(path);
            stream.Write(json);
            stream.Close();
        }
        else
        {
            var stream = File.CreateText(path);
            stream.Write(json);
            stream.Close();
        }
    }
}
