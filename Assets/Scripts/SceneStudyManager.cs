using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;


[System.Serializable]
public class StudyBackup
{
    public string optionsFile;
    public int numCompleted;
    public int currentSession;
    public int audioCompleted;
    public int noAudioCompleted;
    public bool audioFirst;
    public Session[] completed;

    public StudyBackup(string o)
    {
        optionsFile = o;
        completed = new Session[Const.NUM_TOTAL];
    }
}
//Log for web app
[System.Serializable]
public class StudyLog
{
    public Vector3 headPos;
    public float angle;
    public long time;
    public int currentSession;
    public bool recordingStatus;
    //public bool trackingStatus;
    public bool interruptionStatus;
    public int numClicks;
    public string lastClick;
    public bool gazeValid;
}

//Gaze and Head pos frame
[System.Serializable]
public struct StudyFrame
{
    public Vector3 hPos;
    public Quaternion hRot;
    public Vector3 gazeOrigin;
    public Vector3 gazeDirection;
    public bool gazeValid;
    public long timestamp;
    public string activeGems;

}

// Details of each session
[System.Serializable]
public struct Session
{
    public int physical;
    public bool audio;
    public int callWord;
    public int audioTrack;
    public string map;

    /*    public int[] wordOrder;
        public double[] delays;*/
}

[System.Serializable]
public struct Track
{
    public double[] delay;
    public int[] wordOrder;
}


// Class to store callwords from file
[System.Serializable]
public class AudioTracks
{
    public List<Track> tracks;
}

[System.Serializable]
public struct Sessions
{
    public string userID;
    public List<Session> sessions;
}

[System.Serializable]
public class Trials
{
    public List<Sessions> trials;
}
//Description of user's study
[System.Serializable]
public class StudyDescription
{
    public string userID;
    public Sessions studySessions;
    public int startSession;
    //public SessionDescription sessionDesc;          //Just storing file data to randomize, not written to json
    public StudyDescription(string path, int idx)
    {

        using (StreamReader myFile = new StreamReader(Path.Combine(path, "user_study_options.json")))
        {
            string json = myFile.ReadToEnd();
            Trials temp = JsonUtility.FromJson<Trials>(json);
            //Debug.Log(temp.trials.Count);
            studySessions = temp.trials[idx];
            userID = temp.trials[idx].userID;
            /*            Debug.Log("DESC USERID");
                        Debug.Log(userID);
                        Debug.Log(studySessions.sessions[0].map);*/
            if (studySessions.sessions.Count != Const.NUM_TOTAL)
            {
                //TODO deal with this
                Debug.Log("INCORRECT NUMBER OF SESSIONS IN FILE!!");
            }
        }


    }
}

//Just to provide structure for firebase
public class SessionLog
{
    public int sessionID;
    //public List<StudyFrame> frames;
    //public string[] collectibles;
    public long goalCollected;
    //public int randomSeed;

    //TODO: NoFramesCopy?

    public SessionLog(int id = 0)
    {
        sessionID = id;
        //frames = new List<StudyFrame>();
        //frames.Add(new StudyFrame());
        //collectibles = new string[10];
        //randomSeed = seed;
    }
}

//Session recording which is written to json
[System.Serializable]
public class SessionRecording
{
    public int sessionID;
    public long startTime;
    public long length;
    public long numFrames;
    public float tickRate;
    public List<StudyFrame> frames;
    public List<StudyAudio> audio;
    public List<StudyClick> clicks;
    public List<Recording> recording;
    public List<Interruption> interruptions;

    public SessionRecording(int id, float t)
    {
        sessionID = id;
        tickRate = t;
        frames = new List<StudyFrame>();
        audio = new List<StudyAudio>();
        clicks = new List<StudyClick>();
        recording = new List<Recording>();
        interruptions = new List<Interruption>();
    }
}

//Json entry for a single user
[System.Serializable]
public class StudyObject
{
    public string _valid = "null";
    public string userID;
    public float tickRate;
    public string optionsFile;
    public StudyDescription description;
    public SessionLog[] sessionLog;
    public SessionRecording[] sessionRecordings;


    public StudyObject(string path, int idx, float tickRate = Const.TICK_RATE)
    {
        this.tickRate = tickRate;
        description = new StudyDescription(path, idx);
        userID = description.userID;
        optionsFile = description.userID;
        /*        Debug.Log("OBJ USERID");
                Debug.Log(userID);*/
        //TODO: other inputs?
        sessionRecordings = new SessionRecording[Const.NUM_TOTAL];
        sessionLog = new SessionLog[Const.NUM_TOTAL];
        for (int i = 0; i < Const.NUM_TOTAL; i++)
        {
            sessionRecordings[i] = new SessionRecording(i, this.tickRate);
            sessionLog[i] = new SessionLog(i);
        }
    }

}

//Click log frame
[System.Serializable]
public struct StudyClick
{
    public long timeStamp;
    public char type;
    public string gems;
    public long frameIndex;
    public StudyClick(char w, long t, long f, string s = "")
    {
        type = w;
        timeStamp = t;
        gems = s;
        frameIndex = f;
    }
}

//Call word log frame
[System.Serializable]
public struct StudyAudio
{
    public int word;
    public long timeStamp;
    public long frameIndex;
    public StudyAudio(int w, long t, long f)
    {
        word = w;
        timeStamp = t;
        frameIndex = f;
    }
}

//Recording status log frame
[System.Serializable]
public struct Recording
{
    public long frameIndex;
    public long timeStamp;
    public bool status;

    public Recording(bool s, long t, long f)
    {
        status = s;
        timeStamp = t;
        frameIndex = f;
    }
}


[System.Serializable]
public struct Interruption
{
    public long frameIndex;
    public long timeStamp;
    public bool status;

    public Interruption(bool s, long t, long f)
    {
        status = s;
        timeStamp = t;
        frameIndex = f;
    }
}

public class SceneStudyManager : MonoBehaviour
{

    //General
    string userID = "";
    static System.Random random = new System.Random();

    string url = "";
    public Camera mainCamera;
    public Camera topDown;
    public GameObject World;
    public GameObject Physical;
    public GameObject SceneContent;
    public GameObject[] Gems;
    public GameObject Timer;
    public Transform target;
    public GameObject[] SceneGems;
    public GameObject EyeGaze;
    public GameObject InputText;
    public GameObject PhysicalOcclusion;
    public Transform PhysicalParent;

    //objects to write to json
    StudyLog log;
    public StudyObject obj;
    StudyBackup backup;
    AudioTracks audioTracks;

    float logTimer = 0f;
    int studyTimer = 0;
    public int currentSession = 0;
    public StudyFrame currentFrame;
    bool recording;
    public Material occlusionMat;

    public long startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

    //Audio related
    public AudioSource[] audio;
    public AudioSource ambient;
    public AudioSource clickSound;
    string[] callWords = Const.CALL_WORDS;
    int currWord = 0;
    bool notPlayedYet = false;

    //Logging counters
    long frameNum = 0;
    long audioNum = 0;
    long clickNum = 0;
    //long proxNum = 0;
    long recNum = 0;
    long intNum = 0;
    double nextWordTime;

    bool clicking = false;
    float totalDownTime = 0;
    bool studyInProgress = false;
    bool getTextInput = true;
    bool changeTrial = false;
    bool interruption = false;


    Session sessionInfo;

    //Logging objects
    public List<StudyFrame> newFrames;
    public List<StudyClick> newClicks;
    public List<StudyAudio> newAudio;
    //public List<ProximityLog> newProx;
    public List<Recording> newRec;

    #region Public methods

    public void CreateOccluders()
    {
        if(PhysicalOcclusion)
        {
            Destroy(PhysicalOcclusion);
        }
        PhysicalOcclusion = GameObject.Instantiate(Physical, Physical.transform.position, Physical.transform.rotation, Physical.transform.parent);
        PhysicalOcclusion.name = "physicalOcclusion";
        var renderers = PhysicalOcclusion.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.material = occlusionMat;
        }
        PhysicalOcclusion.SetActive(!Physical.activeSelf);
    }
    //Generate random string
    public static string GenerateHexString(int digits)
    {
        return string.Concat(Enumerable.Range(0, digits).Select(_ => random.Next(16).ToString("X")));
    }

    public void LoadBackup()
    {
        Debug.Log("load backup");
        if (File.Exists(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json")))
        {
            using (StreamReader myFile = new StreamReader(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json")))
            {
                backup = JsonUtility.FromJson<StudyBackup>(myFile.ReadToEnd());
                //TODO: ADD REQUIRED SESSIONS
                backup.currentSession += 1;
                print("BACKUP");
                print(backup.currentSession);

            }
            using (StreamWriter myNewFile = new StreamWriter(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json")))
            {
                myNewFile.Write(JsonUtility.ToJson(backup));
            }

        }
        else
        {
            obj.description.startSession = 0;
            backup = new StudyBackup(obj.optionsFile);
            backup.currentSession = -1;
            backup.audioCompleted = 0;
            backup.noAudioCompleted = 0;
            backup.numCompleted = 0;
            backup.audioFirst = obj.description.studySessions.sessions[0].audio;
            using (StreamWriter myFile = new StreamWriter(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json")))
            {
                myFile.Write(JsonUtility.ToJson(backup));
            }
        }
        currentSession = backup.currentSession + 1;
        sessionInfo = obj.description.studySessions.sessions[currentSession];
        Debug.Log(currentSession);
        Debug.Log(sessionInfo.audio);
        Debug.Log("start sessions");
        ChangeScene(-1, Int32.Parse(sessionInfo.map));
        studyInProgress = true;
        
    }

    public void StartNewStudy()
    {
        Debug.Log("start new study");
        obj.description.startSession = 0;
        backup = new StudyBackup(obj.optionsFile);
        backup.currentSession = -1;
        backup.audioCompleted = 0;
        backup.noAudioCompleted = 0;
        backup.numCompleted = 0;
        backup.audioFirst = obj.description.studySessions.sessions[0].audio;

        using (StreamWriter myFile = new StreamWriter(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json")))
        {
            myFile.Write(JsonUtility.ToJson(backup));
        }
        currentSession = backup.currentSession + 1;
        sessionInfo = obj.description.studySessions.sessions[currentSession];
        Debug.Log("start sessions");
        ChangeScene(-1, Int32.Parse(sessionInfo.map));
        studyInProgress = true;
    }

    //Initialize the user's data entry in firebase
    public void InitializeUserAllData()
    {
        //Initialize user in firebase
        Dictionary<string, Dictionary<string, string>> requestBody = new Dictionary<string, Dictionary<string, string>>(){
            { userID, new Dictionary<string, string>() {{"log", "null" }} }
        };
        string requestBodyString = JsonUtility.ToJson(requestBody);
        StartCoroutine(Upload(userID, "new", requestBodyString));

        requestBodyString = JsonUtility.ToJson(new Dictionary<string, string>() { { "description", "null" }, { "log", "null" } });
        StartCoroutine(Upload(userID, "desc", requestBodyString));

        requestBodyString = JsonUtility.ToJson(obj);
        StartCoroutine(Upload(userID, "desc", requestBodyString));


        //initalize local frame and audio stores

        newFrames = new List<StudyFrame>();
        newAudio = new List<StudyAudio>();
        newClicks = new List<StudyClick>();
        //newProx = new List<ProximityLog>();
        newRec = new List<Recording>();

    }


    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            userID = uwr.downloadHandler.text;
        }
    }

    //Writes a json string to firebase
    public IEnumerator Upload(string userID, string data, string requestBodyString)
    {
        //https://github.com/Kubos-cz/Unity-WebRequest-Example
        // Create request URL string based on type of data to upload
        if (data.Equals("new"))
        {
            url = Const.PROJECT_PATH + ".json";
        }
        else if (data.Equals("desc"))
        {
            url = Const.PROJECT_PATH + "/" + userID + ".json";
        }
        else if (data.Equals("frame"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + "/frames.json";
        }
        else if (data.Equals("audio"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + "/audio.json";
        }
        else if (data.Equals("log"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/log.json";
        }
        else if (data.Equals("backup"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/backup.json";
        }
        else if (data.Equals("click"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + "/clicks.json";
        }
        else if (data.Equals("rec"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + "/recording.json";
        }
        else if (data.Equals("int"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + "/interruptions.json";
        }
        else if (data.Equals("starttime"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + ".json";
        }
        else if (data.Equals("length"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + ".json";
        }
        else if (data.Equals("numframes"))
        {
            url = Const.PROJECT_PATH + "/" + userID + "/sessionRecordings/" + currentSession + ".json";
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

    //Record data (called every 10 ticks)
    public void SaveStudy()
    {
        currentFrame.timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        currentFrame.hPos = mainCamera.transform.position;
        currentFrame.hRot = mainCamera.transform.rotation;
        currentFrame.gazeOrigin = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin;
        currentFrame.gazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
        currentFrame.gazeValid = CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid;
        currentFrame.activeGems = FindActiveGems();
        //EyeGaze.transform.position = currentFrame.gazeOrigin + currentFrame.gazeDirection;
        obj.sessionRecordings[currentSession].frames.Add(currentFrame);
        //Debug.Log("count: " + obj.sessionRecordings[currentSession].frames.Count.ToString());
        newFrames.Add(currentFrame);
    }

    //Write other saved data too (every second)
    public void LogStudy()
    {

        string requestBodyString = "";
        if (newFrames.Count > 0)
        {
            //data log
            requestBodyString = "";

            for (int i = 0; i < newFrames.Count - 1; i++)
            {
                requestBodyString += "\"" + frameNum + "\":" + JsonUtility.ToJson(newFrames[i]) + ",";
                frameNum++;
            }
            //requestBodyString = "{\"frames\":{" + requestBodyString + "\"" + frameNum + "\":" + JsonUtility.ToJson(newFrames[newFrames.Count - 1]) + "}}";
            requestBodyString = "{" + requestBodyString + "\"" + frameNum + "\":" + JsonUtility.ToJson(newFrames[newFrames.Count - 1]) + "}";
            frameNum++;
            StartCoroutine(Upload(userID, "frame", requestBodyString));
            newFrames = new List<StudyFrame>();
        }

        //log audio
        if (newAudio.Count > 0)
        {
            requestBodyString = "";

            for (int i = 0; i < newAudio.Count - 1; i++)
            {
                requestBodyString += "\"" + audioNum + "\":" + JsonUtility.ToJson(newAudio[i]) + ",";
                audioNum++;
            }
            //requestBodyString = "{\"frames\":{" + requestBodyString + "\"" + frameNum + "\":" + JsonUtility.ToJson(newFrames[newFrames.Count - 1]) + "}}";
            requestBodyString = "{" + requestBodyString + "\"" + audioNum + "\":" + JsonUtility.ToJson(newAudio[newAudio.Count - 1]) + "}";
            audioNum++;
            StartCoroutine(Upload(userID, "audio", requestBodyString));
            newAudio = new List<StudyAudio>();
        }

        //log clicks
        if (newClicks.Count > 0)
        {
            requestBodyString = "";

            for (int i = 0; i < newClicks.Count - 1; i++)
            {
                requestBodyString += "\"" + clickNum + "\":" + JsonUtility.ToJson(newClicks[i]) + ",";
                clickNum++;
            }
            //requestBodyString = "{\"frames\":{" + requestBodyString + "\"" + frameNum + "\":" + JsonUtility.ToJson(newFrames[newFrames.Count - 1]) + "}}";
            requestBodyString = "{" + requestBodyString + "\"" + clickNum + "\":" + JsonUtility.ToJson(newClicks[newClicks.Count - 1]) + "}";
            clickNum++;
            StartCoroutine(Upload(userID, "click", requestBodyString));
            newClicks = new List<StudyClick>();
        }
    }

    //Log for web app
    public void AppLog()
    {

        Vector3 angle = mainCamera.transform.rotation.eulerAngles;
        //Debug.Log(angle);
        // log for app
        log.time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        //Debug.Log(topDown.gameObject);
        topDown.gameObject.SetActive(true);
        log.headPos = topDown.WorldToScreenPoint(target.position);
        topDown.gameObject.SetActive(false);
        log.angle = angle.y;
        log.currentSession = currentSession;
        log.recordingStatus = recording;
        log.interruptionStatus = interruption;
        log.gazeValid = currentFrame.gazeValid;
/*        if ((mainCamera.transform.position.y < (-0.75)) || (mainCamera.transform.position.y > 0.5))
        {
            Debug.Log(mainCamera.transform.position.y);
            log.trackingStatus = false;
        }
        else
        {
            log.trackingStatus = true;
        }*/
        string requestBodyString = JsonUtility.ToJson(log);
        StartCoroutine(Upload(userID, "log", requestBodyString));
    }

    //Reset session descriptors
    public int NewSession(int nextSession)
    {
        recording = false;
        obj.sessionRecordings[currentSession].numFrames = frameNum;
        obj.sessionRecordings[currentSession].length = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        StartCoroutine(Upload(userID, "length", "{\"length\":" + obj.sessionRecordings[currentSession].length.ToString() + "}"));
        StartCoroutine(Upload(userID, "numframes", "{\"numFrames\":" + obj.sessionRecordings[currentSession].numFrames.ToString() + "}"));
        string jsonData = JsonUtility.ToJson(obj);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, userID + "_" + currentSession + "_" + DateTime.Now.ToString("MMdd_HHmmss_tt") + ".json"), jsonData);
        backup.completed[currentSession] = sessionInfo;
        backup.numCompleted += 1;
        if (obj.description.studySessions.sessions[currentSession].audio == true)
        {
            backup.audioCompleted += 1;
        }
        else
        {
            backup.noAudioCompleted += 1;
        }
                
        if ((backup.audioCompleted + backup.noAudioCompleted) == Const.NUM_SESSIONS)
        {
            //StartCoroutine(Upload(userID, "backup", JsonUtility.ToJson(backup)));
            File.Delete(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json"));
            //return 1;
        }
        else if (((backup.audioFirst == true) && (backup.audioCompleted == Const.NUM_TRIALS) && (backup.audioCompleted == backup.numCompleted)) || ((backup.audioFirst == false) && (backup.noAudioCompleted == Const.NUM_TRIALS) && (backup.noAudioCompleted == backup.numCompleted)))
        {
            backup.currentSession = 3;
            currentSession = 4;
        }        
        
        if(currentSession == 4)
        {
            currentSession += 1;
            backup.currentSession += 1;
        }
        else
        {
            currentSession = nextSession;
            backup.currentSession = nextSession - 1;
        }
        print("NEW SESSION");
        print(currentSession);
        StartCoroutine(Upload(userID, "backup", JsonUtility.ToJson(backup)));
        using (StreamWriter myFile = new StreamWriter(Path.Combine(Application.persistentDataPath, obj.optionsFile + "crash.json")))
        {
            myFile.Write(JsonUtility.ToJson(backup));
        }
        logTimer = 0f;
        studyTimer = 0;
        currWord = 0;
        frameNum = 0;
        audioNum = 0;
        clickNum = 0;
        recNum = 0;
        intNum = 0;
        log.numClicks = 0;
        log.lastClick = "";
        //proxNum = 0;
        int oldScene = Int32.Parse(sessionInfo.map);
        sessionInfo = obj.description.studySessions.sessions[currentSession];

        ChangeScene(oldScene, Int32.Parse(sessionInfo.map));
        
        
        return 0;
    }

    /*    // Load audio into the source
        IEnumerator LoadMusic(string songPath, int i)
        {
            //Debug.Log("Loading");
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(songPath, AudioType.WAV))
            {
                ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;

                yield return uwr.SendWebRequest();
                //Debug.Log("Sent request");
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.LogError(uwr.error);
                    yield break;
                }

                DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
                //Debug.Log("handler");
                if (dlHandler.isDone)
                {
                    AudioClip audioClip = dlHandler.audioClip;

                    if (audioClip != null)
                    {
                        if (i == -1)
                            ambient.clip = DownloadHandlerAudioClip.GetContent(uwr);
                        else if (i == -2)
                        {
                            clickSound.clip = DownloadHandlerAudioClip.GetContent(uwr);
                        }

                        else
                        {
                            audio[i].clip = DownloadHandlerAudioClip.GetContent(uwr);

                        }


                        //Debug.Log("Playing song using Audio Source!");

                    }
                    else
                    {
                        Debug.Log("Couldn't find a valid AudioClip :(");
                    }
                }
                else
                {
                    Debug.Log("The download process is not completely finished.");
                }
            }


        }*/

    private async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private async Task CountDown()
    {
        int delay = 10;
        if (currentSession == 5)
        {
            delay = 60;
        }
        Timer.SetActive(true);
        for (int i = delay; i >= 0; i--)
        {
            if (sessionInfo.audio)
            {
                Timer.GetComponent<TextMesh>().text = "Your assigned word is " + Const.CALL_WORDS[sessionInfo.callWord] + "\n" + i.ToString();
            }
            else
            {
                Timer.GetComponent<TextMesh>().text = "There is no need to pay attention to the audio\n" + i.ToString();
            }

            await WaitOneSecondAsync();
        }
        if (Timer.activeSelf)
        {
            Timer.SetActive(false);
        }
        

    }
    public async void ChangeScene(int oldScene, int newScene)
    {

        ambient.Stop();
        if (oldScene != -1)
        {
            World.SetActive(false);
            Gems[oldScene].SetActive(false);


        }

        await CountDown();
        Gems[newScene].SetActive(true);
        World.SetActive(true);
        int i = 0;
        foreach (Transform child in Gems[newScene].transform)
        {
            SceneGems[i] = child.gameObject;
            i++;
        }
        startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (sessionInfo.audio)
            nextWordTime = AudioSettings.dspTime + Const.INITIAL_DELAY + audioTracks.tracks[sessionInfo.audioTrack].delay[currWord];
        StartCoroutine(Upload(userID, "starttime", "{\"startTime\":" + startTime.ToString() + "}"));
        //TODO Change after tutorial is added
        obj.sessionRecordings[currentSession].startTime = startTime;
        recording = true;
        ambient.Play();
        interruption = true;
    }

    public string FindActiveGems()
    {

        string gemNames = "";
        Dictionary<string, float> s = new Dictionary<string, float>();
        for (int i = 0; i < Const.NUM_GEMS; i++)
        {
            Vector3 dist = (SceneGems[i].transform.position - mainCamera.transform.position);
            float angle = Vector3.Angle(mainCamera.transform.forward, dist);

            if (dist.magnitude < Const.GEM_DIST && angle < Const.GEM_ANGLE)
            {
                s.Add(SceneGems[i].name, dist.magnitude);
            }
        }
        List<KeyValuePair<string, float>> myList = new List<KeyValuePair<string, float>>(s);
        myList.Sort(
            delegate (KeyValuePair<string, float> firstPair,
            KeyValuePair<string, float> nextPair)
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
        );
        foreach(KeyValuePair<string, float> elem in myList)
        {
            gemNames += " | " + elem.Key;
        }
        if(gemNames.Equals(""))
        {
            return gemNames;
        }
        log.lastClick = gemNames;
        return gemNames;
    }

    public AudioTracks ReadAudioTracks(string path)
    {
        string json;
        using (StreamReader myFile = new StreamReader(Path.Combine(path, "audio_tracks.json")))
        {
            json = myFile.ReadToEnd();
        }
        AudioTracks audio = JsonUtility.FromJson<AudioTracks>(json);
        return audio;
    }

    public void LoadStudy()
    {
        World.SetActive(true);
        if(InputText.GetComponent<InputField>().text.Length<2)
        {
            return;
        }
        int idx = Int32.Parse(InputText.GetComponent<InputField>().text.Substring(0, 2));
        InputText.SetActive(false);

        if(Physical)
        {
            Physical.transform.parent = PhysicalParent;
        }
        //int idx = 0;
        obj = new StudyObject(Application.streamingAssetsPath, idx);
        userID = obj.optionsFile + DateTime.Now.ToString("MMdd_HHmmss_tt");
        obj.userID = userID;
        Physical = GameObject.Find("Physical" + obj.description.studySessions.sessions[0].physical.ToString("D2"));
        PhysicalParent = Physical.transform.parent;
        Physical.transform.parent = SceneContent.transform;
        CreateOccluders();
        World.SetActive(false);
        StartCoroutine(Upload(userID, "new", "{\"current\":\"" + userID + "\"}"));
        Timer.SetActive(true);
        Timer.GetComponent<TextMesh>().text = "Study " + obj.optionsFile + " loaded.";
        GameObject.Find("GemManager").GetComponent<GemManager>().InitializeGems(userID, obj.description.studySessions.sessions[0].physical);
        Gems = new GameObject[Const.NUM_LAYOUTS];
        for (int i = 0; i < Const.NUM_LAYOUTS; i++)
        {
            Gems[i] = GameObject.Find("Scene" + (i).ToString("D2"));
            Gems[i].SetActive(false);
        }

        
        getTextInput = false;
    }

    #endregion

    #region Unity methods

    public void TogglePhysical()
    {
        Physical.SetActive(!Physical.activeSelf);
        PhysicalOcclusion.SetActive(!Physical.activeSelf);
    }

    public void ToggleWorld()
    {
        World.SetActive(!World.activeSelf);
    }

    public void ToggleTimer()
    {
        Timer.SetActive(!Timer.activeSelf);
    }

    public void ToggleMap()
    {
        bool mapAdj = SceneContent.GetComponent<MapAdjuster>().enabled;
        SceneContent.GetComponent<MapAdjuster>().enabled = !mapAdj;
        Timer.GetComponent<TextMesh>().text = "MapAdjuster " + SceneContent.GetComponent<MapAdjuster>().enabled;
    }

    public void ChangeStudy()
    {
        getTextInput = true;
        InputText.GetComponent<InputField>().text = "";
        InputText.SetActive(true);
        Timer.GetComponent<TextMesh>().text = "Change Study/Trial";
        studyInProgress = false;
    }

    public void LoadTrial()
    {

        if(InputText.GetComponent<InputField>().text.Length < 1)
        {
            return;
        }
        getTextInput = false;
        int idx = Int32.Parse(InputText.GetComponent<InputField>().text.Substring(0, 1));
        InputText.SetActive(false);
        NewSession(idx);
        studyInProgress = true;

    }

    void Awake()
    {
        Debug.Log(Const.NUM_SESSIONS);
        audioTracks = ReadAudioTracks(Application.streamingAssetsPath);
        CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled = true;
        //EyeGaze = GameObject.Find("EyeGaze");

        
        World = GameObject.Find("Projected_World");
        //Physical = GameObject.Find("Physical00");


        Timer = GameObject.Find("Timer");
        //Timer.SetActive(false);
        SceneGems = new GameObject[Const.NUM_GEMS];


        SceneContent = GameObject.Find("SceneContent");
        //SceneContent = GameObject.Find("WLT");
        /*Anchors = GameObject.Find("Anchors");
        AnchorCubes = new GameObject[Const.NUM_ANCHORS];
        for (int i = 0; i < Const.NUM_ANCHORS; i++)
        {
            AnchorCubes[i] = GameObject.Find("Anchor" + (i + 1).ToString("D2"));
        }*/
        mainCamera = Camera.main;
        //GameObject.Find("Trail").SetActive(false);
        topDown = GameObject.Find("TopDown").GetComponent<Camera>();
        target = Camera.main.transform;
        log = new StudyLog();

        InputText = GameObject.Find("InputField");
        //SetSelectedGameObject(InputText);
        Debug.Log(Application.persistentDataPath);
        recording = false;

        //Load audio clips
        audio = new AudioSource[Const.NUM_WORDS];
        for (int i = 0; i < Const.NUM_WORDS; i++)
        {
            audio[i] = GameObject.Find(callWords[i]).GetComponent<AudioSource>();
            audio[i].playOnAwake = false;
        }

        ambient = GameObject.Find("ambient").GetComponent<AudioSource>();
        ambient.playOnAwake = false;
        clickSound = GameObject.Find("click").GetComponent<AudioSource>();
        clickSound.playOnAwake = false;
        Debug.Log("awake");

    }
    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = Const.FRAME_RATE;

    }

    // Update is called once per frame
    void Update()
    {

        if ((getTextInput == true) || (changeTrial == true))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                InputText.GetComponent<InputField>().text += "0";
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InputText.GetComponent<InputField>().text += "1";
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InputText.GetComponent<InputField>().text += "2";
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                InputText.GetComponent<InputField>().text += "3";
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                InputText.GetComponent<InputField>().text += "4";
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                InputText.GetComponent<InputField>().text += "5";
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                InputText.GetComponent<InputField>().text += "6";
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                InputText.GetComponent<InputField>().text += "7";
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                InputText.GetComponent<InputField>().text += "8";
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                InputText.GetComponent<InputField>().text += "9";
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            LoadStudy();
            InitializeUserAllData();
        }

        if (Input.GetKeyDown(KeyCode.I) && (studyInProgress == false))
        {
            LoadTrial();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            LoadBackup();
            Physical.SetActive(false);
            PhysicalOcclusion.SetActive(!Physical.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartNewStudy();
            Physical.SetActive(false);
            PhysicalOcclusion.SetActive(!Physical.activeSelf);
        }

        if (studyInProgress)
        {

            long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;


            //Start/stop interruption
            if (Input.GetKeyDown(KeyCode.F))
            {
                interruption = !interruption;
                print(interruption);
                print("interruption");
                //Log record status
                Interruption val = new Interruption(interruption, timestamp, frameNum + newFrames.Count);
                obj.sessionRecordings[currentSession].interruptions.Add(val);
                StartCoroutine(Upload(userID, "int", "{\"" + intNum + "\":" + JsonUtility.ToJson(val) + "}"));
                intNum++;


            }

            //Start/stop recording
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Change recording");
                recording = !recording;

                if (recording)
                {
                    ambient.Play();
                    if (sessionInfo.audio)
                        nextWordTime = AudioSettings.dspTime + Const.INITIAL_DELAY + audioTracks.tracks[sessionInfo.audioTrack].delay[currWord];
                }
                else
                {
                    ambient.Stop();
                }
                //Log record status
                Recording val = new Recording(recording, timestamp, frameNum + newFrames.Count);
                obj.sessionRecordings[currentSession].recording.Add(val);
                StartCoroutine(Upload(userID, "rec", "{\"" + recNum + "\":" + JsonUtility.ToJson(val) + "}"));
                recNum++;

                
            }

            //Log the data every second
            logTimer += Time.deltaTime;

            if (!recording)
            {
                if (logTimer > 1f)
                {
                    logTimer = 0f;
                    //LogStudy();
                    AppLog();
                }
                return;
            }
            else
            {

                //Record data every 10 ticks
                studyTimer += 1;
                if (studyTimer == 5)
                {
                    studyTimer = 0;
                    SaveStudy();
                }

                //Log data every second
                if (logTimer > 1f)
                {
                    logTimer = 0f;
                    LogStudy();
                    AppLog();
                }

                if (sessionInfo.audio)
                {
                    //Play audio if necessary + log it
                    if ((AudioSettings.dspTime + Const.WORD_DELAY) > nextWordTime)
                    {
                        audio[audioTracks.tracks[sessionInfo.audioTrack].wordOrder[currWord]].PlayScheduled(nextWordTime);
                        StudyAudio val = new StudyAudio(audioTracks.tracks[sessionInfo.audioTrack].wordOrder[currWord], (timestamp + (long)(Const.WORD_DELAY * 1000)), frameNum + newFrames.Count);
                        currWord++;
                        nextWordTime += audioTracks.tracks[sessionInfo.audioTrack].delay[currWord];
                        newAudio.Add(val);
                        obj.sessionRecordings[currentSession].audio.Add(val);

                    }
                }

                if (Input.GetKeyDown(KeyCode.H))
                {
                    StudyClick val = new StudyClick('X', timestamp, frameNum + newFrames.Count, FindActiveGems());
                    newClicks.Add(val);
                    obj.sessionRecordings[currentSession].clicks.Add(val);
                    clickSound.Play();
                    log.numClicks += 1;
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    StudyClick val = new StudyClick('Y', timestamp, frameNum + newFrames.Count, FindActiveGems());
                    newClicks.Add(val);
                    obj.sessionRecordings[currentSession].clicks.Add(val);
                    clickSound.Play();
                    log.numClicks += 1;
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    StudyClick val = new StudyClick('A', timestamp, frameNum + newFrames.Count, FindActiveGems());
                    newClicks.Add(val);
                    obj.sessionRecordings[currentSession].clicks.Add(val);
                    clickSound.Play();
                    log.numClicks += 1;
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    StudyClick val = new StudyClick('B', timestamp, frameNum + newFrames.Count, FindActiveGems());
                    newClicks.Add(val);
                    obj.sessionRecordings[currentSession].clicks.Add(val);
                    clickSound.Play();
                    log.numClicks += 1;
                }

                //Record clicks
                if (Input.GetKeyDown(KeyCode.M))
                {
                    Debug.Log("M");
                    StudyClick val = new StudyClick('T', timestamp, frameNum + newFrames.Count);
                    newClicks.Add(val);
                    obj.sessionRecordings[currentSession].clicks.Add(val);
                    totalDownTime = 0;
                    clicking = true;
                    clickSound.Play();
                    log.numClicks += 1;
                    log.lastClick = "target";
                }

                // If a first click detected, and still clicking,
                // measure the total click time, and fire an event
                // if we exceed the duration specified
                if (clicking && Input.GetKey(KeyCode.M))
                {
                    totalDownTime += Time.deltaTime;

                    if (totalDownTime >= Const.CLICK_DURATION)
                    {
                        Debug.Log("Long click");
                        clicking = false;
                        recording = false;
                        LogStudy();
                        if (NewSession(currentSession + 1) == 1)
                        {
                            //end the study
                            Debug.Log("STUDY COMPLETE");
                            recording = false;
                            string jsonData = JsonUtility.ToJson(obj);
                            File.WriteAllText(Path.Combine(Application.persistentDataPath, userID + ".json"), jsonData);
                        }
                    }
                }

                // If a first click detected, and we release before the
                // duraction, do nothing, just cancel the click
                if (clicking && Input.GetKeyUp(KeyCode.M))
                {
                    clicking = false;
                }

            }

        }
    }


    #endregion
}