using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Text.RegularExpressions;

/*[System.Serializable]
public class StudyData
{
    public SessionData[] sessionRecordings;
    public StudyData()
    {
        sessionRecordings = new SessionData[4];
        for (int i = 0; i < 4; i++)
        {
            sessionRecordings[i] = new SessionData();
        }
    }
}

[System.Serializable]
public class SessionData
{
    public float distance;
    public bool[] QVisited;
    public bool[] Qseen;
    public float[] clickTimes;

    public SessionData()
    {
        distance = 0.0f;
        QVisited = new bool[10];
        Qseen = new bool[10];
        clickTimes = new float[20];
    }
}*/

[System.Serializable]
public struct GemClick
{
    public double timestamp;
    public char click;
    public bool correct;

    public GemClick(double t, char c, bool b)
    {
        timestamp = t;
        click = c;
        correct = b;
    }
}

[System.Serializable]
public class ValidRange
{
    public int init;
    public int end;

    public bool isValid(int index)
    {
        if (index >= init && index < end)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


public class StudyReader : MonoBehaviour
{

    public bool coverage;
    public bool view;
    public bool trail;
    public bool JPGmap;
    public bool EXRmap;
    public List<string> fileList;
    public int currentFileIndex = 0;
    public bool useFireBase = false;
    public string filepath;
    public string folderpath;
    public int startSessionIndex = 0;
    public Transform head;
    public StudyObject studyObject;
    public SessionRecording fc;
    public int frameIndex = 0;
    public float tickRate = Const.TICK_RATE;
    public bool click;
    public bool trigger;
    public long startTime;

    private float t = 0.0f;
    private string jsonString;
    private bool loadFromJson;
    private bool startReplay;
    public bool interpolate = true;
    StudyFrame sf0;
    StudyFrame sf1;
    public float timeScale = 1f;
    private float lastTimeScale;

    public Texture2D totalMap;
    public Texture2D coverageMap;
    public Texture2D viewMap;
    public Texture2D trailMap;
    public LayerMask coverageOnly;
    public LayerMask viewOnly;
    public LayerMask mapOnly;
    public LayerMask trailOnly;
    public LayerMask rayMask;
    public RenderTexture rt;
    public Shader coverageShader;
    public ParticleSystem coverageParticles;
    public ParticleSystem viewParticles;
    private Camera headCam;
    private List<ParticleSystem.Particle> vparticles;
    private List<ParticleSystem.Particle> cparticles;
    //public StudyData studyData;

    public Camera mainCamera;
    public Camera topDown;
    public GameObject World;
    public GameObject SceneContent;
    public GameObject Anchors;
    public GameObject[] AnchorCubes;
    public int currentAnchor = -1;
    public GameObject[] Gems;
    public GameObject Timer;
    public Transform target;
    public GameObject[] SceneGems;
    public GameObject EyeGaze;
    public GameObject Trail;

    public TextMesh VisualizerClock;
    public TextMesh VisualizerPrompt;
    public TextMesh VisualizerClick;

    private Vector3[] debugHits = new Vector3[20];
    private int currentClickIndex = 0;

    public float distTh = 2f;
    public float angleTh = 30f;
    public bool writeJSON = true;
    public bool showValidationGizmos = false;

    public int currentScene;
    public int currentSession;
    public AudioTracks audioTracks;
    public List<StudyAudio> audioStream;
    public List<StudyClick> clickStream;
    public int audioIndex;
    public int clickIndex;
    public long nextWordTime;
    public long nextClickTime;
    public float timeStamp;
    private float playbackTime;
    public int wordTimer;
    public int clickTimer;
    public char lastClick;
    public int lastWord;
    public int correctGems;
    public int correctAudio;
    public int wrongAudio;
    public int totalGems;
    public int totalAudio;

    public int correctPhysicalGems;
    public int totalPhysicalGems;

    public int correctVirtualGems;
    public int totalVirtualGems;

    public int correctFloatingGems;
    public int totalFloatingGems;


    public int visitedPhisycalGems;
    public int visitedVirtualGems;
    public int visitedFloatingGems;


    public int wrongPhysicalGems;
    public int wrongVirtualGems;
    public int wrongFloatingGems;


    public int repeatedPhysicalGems;
    public int repeatedVirtualGems;
    public int repeatedFloatingGems;


    public float totalTimeToDecide;
    public float timeToDecide;
    public GameObject gemUnderConsideration;

    private float setframeTimer = 0f;

    public StudyFrame currentFrame;
    public StudyFrame nextFrame;

    public Dictionary<int, bool> audioTried;
    public Dictionary<string, int> gemVisited;
    public Dictionary<string, int> gemCorrect;
    public Dictionary<string, List<GemClick>> gemClassification;

    public Dictionary<char, string> KEY_MAP = new Dictionary<char, string> {
        {'X', "GemHorizontal.*Smooth.*" },
        {'Y', "GemVertical.*Smooth.*" },
        {'A', "GemVertical.*Rough.*" },
        {'B', "GemHorizontal.*Rough.*" }
    };

    public Regex rgx;
    //Audio related
    public AudioSource[] audio;
    public AudioSource ambient;
    string[] callWords = Const.CALL_WORDS;



    public StudySummary summary;

    public GameObject playButton;
    public GameObject pauseButton;
    public Slider slider;
    public Dropdown dropdown;
    public Dropdown fileDropDown;
    public Text timeText;
    public InputField folderpathUI;
    public ControllerViz contrellerviz;
    public GameObject interruptUI;
    public bool interrupted;
    public List<ValidRange> validRanges;
    public GameObject lastActiveGemUI;
    public GameObject gemUnderConsiderationUI;
    public Text summaryText;

    public float distanceTraveled;
    private Vector3 lastpos;
    private Quaternion lastRot;
    private int lastFrameTime;
    private bool lastinterrupted;
    public float rotationMetric;
    public float totalSessionTime;
    public bool autoProceed;
    

    public void CheckValidRanges()
    {
        validRanges = new List<ValidRange>();
        for (int i = 0; i < fc.interruptions.Count; i++)
        {
            if (fc.interruptions[i].status == false)
            {
                ValidRange vr = new ValidRange();
                vr.init = (int)fc.interruptions[i].frameIndex;
                validRanges.Add(vr);
            }
            else
            {
                validRanges[validRanges.Count - 1].end = (int)fc.interruptions[i].frameIndex;
            }
        }
    }

    public bool IsFrameUninterrupted(int index)
    {
        for (int i = 0; i < validRanges.Count; i++)
        {
            if (validRanges[i].isValid(index))
            {
                return true;
            }
        }
        
        return false;
    }

    

    IEnumerator LoadMusic(string songPath, int i)
    {
        //print("Loading");
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(songPath, AudioType.WAV))
        {
            ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;

            yield return uwr.SendWebRequest();
            //print("Sent request");
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
            //print("handler");
            if (dlHandler.isDone)
            {
                AudioClip audioClip = dlHandler.audioClip;

                if (audioClip != null)
                {
                    if (i == -1)
                        ambient.clip = DownloadHandlerAudioClip.GetContent(uwr);
                    else
                    {
                        audio[i].clip = DownloadHandlerAudioClip.GetContent(uwr);

                    }


                    Debug.Log("Playing song using Audio Source!");

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


    }

    public AudioTracks ReadAudioTracks(string path)
    {
        string json;
        using (StreamReader myFile = new StreamReader(Path.Combine(path, "audio_tracks.json")))
        {
            json = myFile.ReadToEnd();
        }
        AudioTracks audio = JsonUtility.FromJson<AudioTracks>(json);
        Debug.Log(audio.tracks[0].delay[0]);
        return audio;
    }

    public bool CheckClick(char click, string gemName)
    {
        rgx = new Regex(KEY_MAP[click]);
        if (rgx.IsMatch(gemName))
            return true;
            
        return false;
    }

    public bool CheckAudio()
    {
        if (lastWord == studyObject.description.studySessions.sessions[currentSession].callWord)
            return true;
        return false;
    }

    public void LoadNextFile()
    {
        currentFileIndex++;
        LoadFile(currentFileIndex);
    }


    public void LoadFile(int index)
    {
        currentFileIndex = index;
        if (currentFileIndex < fileList.Count)
        {
            filepath = fileList[currentFileIndex];
            if (!useFireBase)
            {
                jsonString = File.ReadAllText(Path.Combine(folderpath, filepath + ".json"));
                LoadFromJSON();
                dropdown.SetValueWithoutNotify(0);
            }
            else
            {
                //DownloadFromFireBase(filepath + ".json");
            }
        }
        else
        {
            Debug.Log("End of all files...");
            Debug.Break();
        }
    }

    public void LoadScene(int index)
    {
        currentSession = index;
        int newScene = Int32.Parse(studyObject.description.studySessions.sessions[currentSession].map);

        Debug.Log("sceneindex " + currentScene);
        fc = studyObject.sessionRecordings[currentSession];
        audioStream = fc.audio;
        clickStream = fc.clicks;
        tickRate = Const.TICK_RATE;
        InitScene();
        print(studyObject.description.studySessions.sessions);

        ChangeScene(currentScene, newScene);
        currentScene = newScene;
        cparticles = new List<ParticleSystem.Particle>();
        vparticles = new List<ParticleSystem.Particle>();
        dropdown.SetValueWithoutNotify(currentSession);
        CheckValidRanges();
        ComputeGems();
        summaryText.text = $"Physical Gems:{correctPhysicalGems}/{totalPhysicalGems}\nVirtual Gems:{correctVirtualGems}/{totalVirtualGems}\nFloating Gems:{correctFloatingGems}/{totalFloatingGems}\n";
    }

    public void ComputeGems()
    {
        totalPhysicalGems = 0;
        totalVirtualGems = 0;
        totalFloatingGems = 0;


        foreach (Transform t in Gems[currentScene].transform)
        {
            var clicks = gemClassification[t.name];
            
            if (t.name.ToLower().Contains("physical01"))
            {
                totalPhysicalGems++;

            }
            else if(t.name.ToLower().Contains("physical00"))
            {
                totalVirtualGems++;

            }
            else if (t.name.ToLower().Contains("none"))
            {
                totalFloatingGems++;
            }

        }

     //gemClassification[name].Add(new GemClick(clickStream[clickIndex].timeStamp, lastClick, CheckClick(lastClick, gems[1])));

    }

    /*private void SceneManager_sceneLoaded(Scene sc, LoadSceneMode arg1)
    {
        SceneManager.SetActiveScene(sc);
        collector.Arrange(fc.randromSeed, fc.id);
        SetQuestions();
        startReplay = true;
        quizAsked = false;
        currentScene = sc;
        currentClickIndex = 0;
        frameIndex = 0;

        if (fc.frames.Count == 0)
        {
            fc = studyObject.sessionRecordings[currentSession + 1];
        }

        int lastAdd = 0;
        for (int i = 0; i < currentSession; i++)
        {
            lastAdd += studyObject.sessionRecordings[i].frames.Count;
        }
        Debug.Log("last add " + lastAdd);
        Debug.Log("frames count " + fc.frames.Count);

        if (fc.frames.Count > lastAdd + 2000)
        {
            Debug.Log("start from " + lastAdd);
            frameIndex = lastAdd + 1;
        }
    }*/


    public void Play()
    {
        startReplay = true;
        playButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void Pause()
    {
        startReplay = false;
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }
    void InitScene()
    {
        frameIndex = 0;
        audioIndex = 0;
        clickIndex = 0;
        nextClickTime = clickStream[clickIndex].timeStamp;
        wordTimer = 0;
        clickTimer = 0;

        correctAudio = 0;
        correctGems = 0;
        totalAudio = 0;
        totalGems = 0;
        distanceTraveled = 0.0f;
        rotationMetric = 0.0f;
        totalSessionTime = 0.0f;

        totalPhysicalGems = 0;
        totalVirtualGems = 0;
        totalFloatingGems = 0;

        correctPhysicalGems = 0;
        correctVirtualGems = 0;
        correctFloatingGems = 0;

        visitedPhisycalGems = 0;
        visitedVirtualGems = 0;
        visitedFloatingGems = 0;

        wrongPhysicalGems = 0;
        wrongVirtualGems = 0;
        wrongFloatingGems = 0;

        repeatedPhysicalGems = 0;
        repeatedVirtualGems = 0;
        repeatedFloatingGems = 0;

        audioTried = new Dictionary<int, bool>();
        gemCorrect = new Dictionary<string, int>();
        gemVisited = new Dictionary<string, int>();
        gemClassification = new Dictionary<string, List<GemClick>>();

        if (fc.frames.Count < 2)
        {
            print("no frames");
            fc = studyObject.sessionRecordings[currentSession + 1];
            currentSession += 1;
           
        }
        else
        {
            //tickRate = (float)Math.Round((1000f / (fc.frames[1].timestamp - fc.frames[0].timestamp)), 0);
            sf0 = fc.frames[0];
            sf1 = fc.frames[1];

        }
    }

    void NextScene()
    {
        currentSession++;
        if (currentSession > Const.NUM_SESSIONS)
        {
            LoadNextFile();
            return;
        }


        //TODO: CHANGE FOR NEW DATA
        LoadScene(currentSession);
    }


    private async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private async Task CountDown()
    {
        //Timer.SetActive(true);
        for (int i = 10; i >= 0; i--)
        {
            if (studyObject.description.studySessions.sessions[currentSession].audio)
            {
                Timer.GetComponent<TextMesh>().text = "Your next assigned word is " + Const.CALL_WORDS[studyObject.description.studySessions.sessions[currentSession].callWord] + "\n" + i.ToString();
            }
            else
            {
                Timer.GetComponent<TextMesh>().text = "There is no need to pay attention to the audio\n" + i.ToString();
            }

            await WaitOneSecondAsync();
        }
        //Timer.SetActive(false);

    }
    public async void ChangeScene(int oldScene, int newScene)
    {
        if (oldScene != -1)
        {
            World.SetActive(false);
            Gems[oldScene].SetActive(false);


        }

        //await CountDown();
        print(newScene);
        Gems[newScene].SetActive(true);
        World.SetActive(true);
        int i = 0;
        foreach (Transform child in Gems[newScene].transform)
        {
            SceneGems[i] = child.gameObject;
            gemVisited[child.gameObject.name] = 0;
            gemClassification[child.gameObject.name] = new List<GemClick>();
            i++;
        }
        playbackTime = 0.0f;
        //startTime = Time.time();
        if(fc.audio.Count > 0)
            nextWordTime = audioStream[audioIndex].timeStamp;
        if(fc.clicks.Count > 0)
            nextClickTime = clickStream[clickIndex].timeStamp;
        Play();
    }
    void LoadFromJSON()
    {
        studyObject = JsonUtility.FromJson<StudyObject>(jsonString);
        GameObject.Find("GemManager").GetComponent<GemManager>().InitializeGems(studyObject.userID, studyObject.description.studySessions.sessions[0].physical);
        Gems = new GameObject[Const.NUM_LAYOUTS];
        for (int i = 0; i < Const.NUM_LAYOUTS; i++)
        {
            Gems[i] = GameObject.Find("Scene" + (i).ToString("D2"));
            Gems[i].SetActive(false);
        }
        print(studyObject.sessionRecordings.Length);
        //studyData = new StudyData();
        currentSession = startSessionIndex - 1;
        NextScene();
        print("next scene");
        print(currentSession);

        for(int i=0;i< studyObject.sessionRecordings.Length; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData("Sess " + i));
        }
        dropdown.SetValueWithoutNotify(0);

        summary.UpdateFromDescription(studyObject.description);

    }

    private void Awake()
    {
        audioTracks = ReadAudioTracks(Application.streamingAssetsPath);
        World = GameObject.Find("Projected_World");
        World.SetActive(false);
        Timer = GameObject.Find("Timer");
        //Timer.SetActive(false);
        SceneGems = new GameObject[Const.NUM_GEMS];
        

        SceneContent = GameObject.Find("SceneContent");
        headCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        headCam.aspect = 1.46f;
        headCam.fieldOfView = 42.44f;
        topDown = GameObject.Find("TopDown").GetComponent<Camera>();
        topDown.gameObject.SetActive(false);
        head = GameObject.Find("Main Camera").GetComponent<Transform>();
        EyeGaze = GameObject.Find("EyeGaze");

        EyeGaze = GameObject.Find("EyeGaze");
        Trail = GameObject.Find("Trail");
        //Trail.SetActive(false);
       
        VisualizerClock = GameObject.Find("VisualizerClock").GetComponent<TextMesh>();
        VisualizerPrompt = GameObject.Find("VisualizerPrompt").GetComponent<TextMesh>();
        VisualizerClick = GameObject.Find("VisualizerClick").GetComponent<TextMesh>();
        VisualizerClock.text = "";
        VisualizerPrompt.text = "";
        VisualizerClick.text = "";
        /*        if (Const.FIRST_PERSON_PLAYBACK == true)
                {
                    print("FIRST_PERSON");
                    headCam.gameObject.tag = "MainCamera";
                }
                else
                {
                    print("THIRD_PERSON");
                    topDown.gameObject.tag = "MainCamera";
                }*/


        audioStream = new List<StudyAudio>();
        clickStream = new List<StudyClick>();
        //Load audio clips
        audio = new AudioSource[Const.NUM_WORDS];
        for (int i = 0; i < Const.NUM_WORDS; i++)
        {

            audio[i] = GameObject.Find(callWords[i]).GetComponent<AudioSource>();
            audio[i].playOnAwake = false;

        }

        CheckValidRanges();


    }


    public void SetFolderPath(string fp)
    {
        folderpath = fp;
        PlayerPrefs.SetString("StudyFolderPath",folderpath);
        LoadFolder();
    }

    public void LoadFolder()
    {
        Regex regex = new Regex(".*json$");
        var matches = Directory.EnumerateFiles(@folderpath).Where(f => regex.IsMatch(f));

        fileList = new List<string>();
        foreach (var match in matches)
        {
            string fpath = Path.GetFileName(match).Replace(".json", "");
            fileList.Add(fpath);
            
        }
        fileDropDown.AddOptions(fileList);
        fileDropDown.SetValueWithoutNotify(0);
        LoadFile(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentScene = 0;
        startReplay = false;

        if (PlayerPrefs.HasKey("StudyFolderPath"))
        {
            folderpath = PlayerPrefs.GetString("StudyFolderPath");
            folderpathUI.SetTextWithoutNotify(folderpath);
            LoadFolder();
        }

        //LoadNextFile();
        //print("loaded file");
        //LoadFolder();
        //Application.targetFrameRate = 60;
    }

    public int NextClickIndex()
    {
        if(clickIndex>= clickStream.Count)
        {
            return (int)clickStream[clickIndex - 1].frameIndex;
        }else 

        if (clickStream.Count > clickIndex)
        {
            return (int)clickStream[clickIndex].frameIndex;
        }else
        {
            return (int)clickStream[clickIndex].frameIndex;
        }
            
    }

    float Qnorm(Quaternion q)
    {
        return Mathf.Sqrt((q.x * q.x) + (q.y * q.y) + (q.z * q.z) + (q.w * q.w));
    }

    Quaternion Qadd(Quaternion q1, Quaternion q2)
    {
        return new Quaternion(q1.x + q2.x, q1.y + q2.y, q1.z + q2.z, q1.w + q2.w);
    }

    Quaternion Qsub(Quaternion q1, Quaternion q2)
    {
        return new Quaternion(q1.x - q2.x, q1.y - q2.y, q1.z - q2.z, q1.w - q2.w);
    }

    float Phi2 (Quaternion q1,Quaternion q2)
    {
        return Mathf.Min(Qnorm(Qsub(q2, q1)), Qnorm(Qadd(q2, q1)));
    }

    public void Step()
    {


        interrupted = !IsFrameUninterrupted(frameIndex);
        interruptUI.SetActive(interrupted);

        t += Time.deltaTime;
        Interpolate();

        if (lastinterrupted != interrupted)
        {
            lastpos = head.position;
            lastRot = head.rotation;
            lastFrameTime = (int)fc.frames[frameIndex].timestamp;
        }
        lastinterrupted = interrupted;


        float frameRotation = Phi2(lastRot, head.rotation);


        if (!interrupted)
        {
            distanceTraveled += (lastpos - head.position).magnitude;
            rotationMetric += frameRotation;
            totalSessionTime += (float)(fc.frames[frameIndex].timestamp - lastFrameTime) / 1000.0f;
        }

        lastpos = head.position;
        lastRot = head.rotation;
        lastFrameTime = (int)fc.frames[frameIndex].timestamp;

        /*
        for (int i = 0; i < SceneGems.Length; i++)
        {
            Vector3 dist = (SceneGems[i].transform.position - head.position);
            float angle = Vector3.Angle(head.forward, dist);
            if (dist.magnitude < 4f && angle < 180)
            {
                currentGem = gem;
                gemInVicinity = true;
                break;
            }
        }
        */


        TimeSpan ts = TimeSpan.FromMilliseconds(timeStamp);
        timeText.text = $"Time: { string.Format("{0:D2}m:{1:D2}s:{2:D3}ms", ts.Minutes, ts.Seconds, ts.Milliseconds)} \nFrame: {frameIndex}\nNext Click: {NextClickIndex()}\nClick Index: {clickIndex}";

        summaryText.text = $"Physical Gems: {correctPhysicalGems}/{totalPhysicalGems}\nVirtual Gems: {correctVirtualGems}/{totalVirtualGems}\nFloating Gems: {correctFloatingGems}/{totalFloatingGems}\n"+
            $"Visited Physical: {visitedPhisycalGems}/{totalPhysicalGems}\n Visited Virtual: {visitedVirtualGems}/{totalVirtualGems}\n Visited Floating: {visitedFloatingGems}/{totalFloatingGems}\n" +
            $"Repeated Physical: {repeatedPhysicalGems}\n Repeated Virtual: {repeatedVirtualGems}\n Repeated Floating: {repeatedFloatingGems}\n" +
            $"Wrong Physical: {wrongPhysicalGems}/{totalPhysicalGems}\nWrong Virtual: {wrongVirtualGems}/{totalVirtualGems}\nWrong Floating: {wrongFloatingGems}/{totalFloatingGems}\n" +
            $"Total Distance: {distanceTraveled.ToString("#.##")}m\nFrame Rotation: {frameRotation.ToString("#.######")}\nTotal Rotation: {rotationMetric.ToString("#.##")}\n" +
            $"Audio Task: {correctAudio}/{totalAudio}\nTotal Time: {totalSessionTime.ToString("#.#")}";

        //tickRate = (float)Math.Round((1000f / (sf1.timestamp - sf0.timestamp)), 0);
        //if (t > 1f / tickRate)
        /*
        if (timeStamp > fc.frames[frameIndex+1].timestamp)
        {
        */
            /*
            int fpf = Mathf.FloorToInt(t / (1f / tickRate));
            fpf = 1;
            for (int k = 0; k < fpf; k++)
            //for (int k = 0; k < 1; k++)
            {*/
                frameIndex++;
                if (frameIndex < fc.frames.Count - 1)
                {

                    sf0 = fc.frames[frameIndex];
                    sf1 = fc.frames[frameIndex + 1];

                    //Timer.GetComponent<TextMesh>().text = ((int)(sf0.timestamp / 1000)).ToString() +"  " + ((int)((timeStamp)/1000)).ToString();
                    Timer.GetComponent<TextMesh>().text = "";
                    VisualizerClock.text = ((int)(sf0.timestamp / 1000)).ToString() + "  " + ((int)((timeStamp) / 1000)).ToString();
                    if (coverage)
                    {
                        ParticleSystem.Particle cPart = new ParticleSystem.Particle();
                        cPart.position = sf0.hPos;
                        cPart.startColor = new Color(1.0f, 1.0f, 1.0f, (float)(sf1.timestamp - sf0.timestamp));
                        cPart.startSize = 4f;
                        cparticles.Add(cPart);

                        coverageParticles.SetParticles(cparticles.ToArray());
                    }

                    if (view)
                    {
                        for (float x = 0f; x < 5f; x++)
                        {
                            for (float y = 0f; y < 4f; y++)
                            {
                                Vector3 p = new Vector3(x / 5f, y / 4f, 1f);
                                Ray r = headCam.ViewportPointToRay(p);
                                RaycastHit hit;
                                if (Physics.Raycast(r, out hit, 100f, rayMask))
                                {
                                    ParticleSystem.Particle vPart = new ParticleSystem.Particle();
                                    vPart.position = hit.point;
                                    vPart.startColor = new Color(1.0f, 1.0f, 1.0f, 0.01f);
                                    vPart.startSize = 1f;
                                    vparticles.Add(vPart);
                                }
                            }

                        }


                        viewParticles.SetParticles(vparticles.ToArray());
                    }



                }
            


            t = 0f;

            if (timeStamp > (nextWordTime - (long)(Const.WORD_DELAY * 1000)) && (audioIndex < audioStream.Count))
            {
                if ((audioStream[audioIndex].timeStamp) >= 0)
                {
                    //audio[audioStream[audioIndex].word].PlayScheduled(nextWordTime);
                    lastWord = audioStream[audioIndex].word;
                    //VisualizerPrompt.text = callWords[audioStream[audioIndex].word];
                    if (audioStream[audioIndex].word == studyObject.description.studySessions.sessions[currentSession].callWord &&!interrupted)
                    {
                        totalAudio++;
                    }
                        
                    audioIndex++;
                    if (audioIndex < audioStream.Count)
                        nextWordTime = audioStream[audioIndex].timeStamp;
                    else
                        print("audio done");
                    wordTimer = 1;

                }
                else
                {
                    audioIndex++;
                    if (audioIndex < audioStream.Count)
                        nextWordTime = audioStream[audioIndex].timeStamp;
                }

            }

            if ((timeStamp > nextClickTime) && (clickIndex < clickStream.Count))
            {
                Debug.Log($"timestamp: {timeStamp},nextclicktime: {nextClickTime}");
                lastClick = clickStream[clickIndex].type;

                

                if (lastClick == 'X')
                {
                    contrellerviz.ButtonPressed(0);
                }
                else if (lastClick == 'Y')
                {
                    contrellerviz.ButtonPressed(1);
                }
                else if (lastClick == 'B')
                {
                    contrellerviz.ButtonPressed(2);
                }
                else if (lastClick == 'A')
                {
                    contrellerviz.ButtonPressed(3);
                }
                else if (lastClick == 'T')
                {
                    contrellerviz.ButtonPressed(4);//trigger
                }

                if (lastClick == 'T')
                {
                if (!audioTried.ContainsKey(audioIndex) && !interrupted)
                {
                    if (CheckAudio())
                    {
                        correctAudio++;
                    }
                    else { wrongAudio++; }
                    audioTried.Add(audioIndex, true);
                }


            }
                else
                {
                    print(clickStream[clickIndex].gems);
                    print(lastClick);
                    string[] gems = clickStream[clickIndex].gems.Split(new string[] { " | " }, StringSplitOptions.None);
                    if (gems.Length > 1)
                    {
                        string name = gems[1];
                        print("gem name :" + name);
                        var g = GameObject.Find(gems[1]);
                        lastActiveGemUI.transform.position = g.transform.position + new Vector3(0f, 0.3f, 0f);
                        gemVisited[name]++;
                        gemClassification[name].Add(new GemClick(clickStream[clickIndex].timeStamp, lastClick, CheckClick(lastClick, gems[1])));

                        var gemC = gemClassification[name];

                        if (gemVisited[name] >= 1)
                        {
                            totalGems++;
                            if (name.ToLower().Contains("physical"))
                            {
                            int physindex = int.Parse(name.ToLower().Substring(name.ToLower().IndexOf("physical")+8,2));
                                
                                if (summary.sessionSummaries[currentSession].physical == physindex)
                                {
                                    if (gemVisited[name] == 1)
                                    {
                                        visitedPhisycalGems++;
                                    }else if(gemVisited[name] > 1)
                                    {
                                        repeatedPhysicalGems++;
                                    }
                                    
                                    if (gemC[gemC.Count - 1].correct)
                                    {

                                        if (CountCorrect(gemC) == 1)
                                        {
                                            correctPhysicalGems++;
                                        }  
                                    }
                                    else
                                    {
                                        wrongPhysicalGems++;
                                    }
                                }
                                else
                                {
                                    if (gemVisited[name] == 1)
                                    {
                                        visitedVirtualGems++;
                                    }
                                    else if (gemVisited[name] > 1)
                                    {
                                        repeatedVirtualGems++;
                                    }
                                if (gemC[gemC.Count - 1].correct)
                                    {
                                        if (CountCorrect(gemC) == 1)
                                        {
                                            correctVirtualGems++;
                                        }
                                    }
                                    else
                                    {
                                        wrongVirtualGems++;
                                    }
                                }
   
                            }
                            else if (name.ToLower().Contains("none"))
                            {
                                if (gemVisited[name] == 1)
                                {
                                    visitedFloatingGems++;
                                }
                                else if (gemVisited[name] > 1)
                                {
                                    repeatedFloatingGems++;
                                }
                            if (gemC[gemC.Count - 1].correct)
                                {
                                    if (CountCorrect(gemC) == 1)
                                    {
                                        correctFloatingGems++;
                                    }
                                }
                                else
                                {
                                    wrongFloatingGems++;
                                }
                            }
                        }
                            
                        


                        //gemClassification.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(print);
                        /*
                        X->h->0
                        A->g->3
                        B->j->2
                        Y->i->1
                        T->4
                        */


                        /*
                            {'X', "GemHorizontal.*Smooth.*" },
                            {'Y', "GemVertical.*Smooth.*" },
                            {'A', "GemVertical.*Rough.*" },
                            {'B', "GemHorizontal.*Rough.*" }
                         */


                        //TODO: Count total click attempts
                        if (CheckClick(lastClick, gems[1]))
                        {
                            gemCorrect[name] = 1;
                            correctGems++;
                        }
                    }

                }
                print($"clickstream count {clickStream.Count},click index: {clickIndex}");

                if (clickIndex < clickStream.Count-1)
                {
                    clickIndex++;
                    nextClickTime = clickStream[clickIndex].timeStamp;
                    print($"next click time {nextClickTime},next click index: {clickIndex}");
                }
                else
                {
                    print("done");
                }
                clickTimer = 1;

            }



            /*                if (wordTimer > 0)
                            {
                                if (wordTimer == 7)
                                {
                                    VisualizerPrompt.text = "";
                                    wordTimer = 0;
                                }
                                else
                                {
                                    wordTimer += 1;
                                }

                            }

                            if (clickTimer > 0)
                            {
                                if (clickTimer == 7)
                                {
                                    VisualizerClick.text = "";
                                    clickTimer = 0;
                                }
                                else
                                {
                                    clickTimer += 1;
                                }

                            }*/

        
        VisualizerClick.text = String.Format("Gems classified: {0:d}\nGems correctly classified: {1:d}\n Targets heard: {2:d}\n Targets detected: {3:d}", totalGems, gemCorrect.Sum(x => x.Value), totalAudio, correctAudio);
        //tickRate = (float)Math.Round((1000f / (sf1.timestamp - sf0.timestamp)), 0);
        //print(tickRate);



        //EyeGaze.transform.position = Vector3.Lerp(sf0.gazeOrigin + sf0.gazeDirection, sf0.gazeOrigin + sf1.gazeDirection, lerp);
        

        if (frameIndex == 2)
        {
            Trail.GetComponent<TrailRenderer>().Clear();
        }
    }

    public int CountCorrect(List<GemClick> clicks)
    {
        int correctCount = 0;
        for (int n = 0; n < clicks.Count; n++)
        {
            if (clicks[n].correct)
            {
                correctCount++;
            }
        }
        return correctCount;
    }
    public void Interpolate()
    {
        float lerp;

        lerp = (float)(timeStamp - sf0.timestamp) / (sf1.timestamp - sf0.timestamp);
        if (!interpolate)
        {
            lerp = 0;
        }

        head.transform.position = Vector3.Lerp(sf0.hPos, sf1.hPos, lerp);
        head.transform.rotation = Quaternion.Lerp(sf0.hRot, sf1.hRot, lerp);
        EyeGaze.transform.position = Vector3.Lerp(sf0.hPos + sf0.gazeDirection, sf1.hPos + sf1.gazeDirection, lerp);
    }
    

    public void ValidateClickTime()
    {
        for (int i = 0; i < clickStream.Count; i++)
        {
            
            if (clickStream[i].timeStamp > playbackTime)
            {
                clickIndex = i;
                Debug.Log("validated");
                nextClickTime = clickStream[i].timeStamp;
                return;
            }
            
        }
    }

    public void SetFrameSlider(float val)
    {
        int index = (int)(val * fc.numFrames);
        SetFrame(index);
    }


    public void SetFrame(int index)
    {
        ValidateClickTime();
        frameIndex = index;
        playbackTime = fc.frames[index].timestamp;
        Step();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastTimeScale != timeScale)
        {
            lastTimeScale = timeScale;
        }




        
        if (startReplay)
        {
            playbackTime += Time.deltaTime * 1000;//System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            timeStamp = playbackTime;

            var breakloop = 0;
            while (timeStamp > fc.frames[frameIndex+1].timestamp)
            {
                breakloop++;
                Step();
                float val = (float)frameIndex / (float)fc.frames.Count;
                slider.SetValueWithoutNotify(val);
                if (breakloop > 100)
                {
                    break;
                }
                if (frameIndex >= studyObject.sessionRecordings[currentSession].frames.Count - 1)
                {
                    WrapSession();
                    return;
                }
            }
            Interpolate();

            if (frameIndex >= studyObject.sessionRecordings[currentSession].frames.Count - 1)
            {
                WrapSession();
                return;
            }

            setframeTimer += Time.deltaTime;
        }

        /* if (studyObject.sessionRecordings[currentSession].frames[frameIndex].timestamp > 300f)
         {
             WrapSession();
         }*/
        
    }

    private void WrapSession()
    {
        topDown.gameObject.SetActive(true);
        totalMap = ScreenTexture(topDown, rt, coverageShader, mapOnly);
        coverageMap = ScreenTexture(topDown, rt, null, coverageOnly);
        viewMap = ScreenTexture(topDown, rt, null, viewOnly);
        trailMap = ScreenTexture(topDown, rt, null, trailOnly);
        topDown.gameObject.SetActive(false);


        if(JPGmap)
        {
            var bytes = totalMap.EncodeToJPG();
            File.WriteAllBytes(folderpath + "/mapsJPG/" + filepath + "_" + currentSession + "_map.jpg", bytes);
            bytes = coverageMap.EncodeToJPG();
            File.WriteAllBytes(folderpath + "/mapsJPG/" + filepath + "_" + currentSession + "_coverage.jpg", bytes);
            bytes = viewMap.EncodeToJPG();
            File.WriteAllBytes(folderpath + "/mapsJPG/" + filepath + "_" + currentSession + "_view.jpg", bytes);
        }
        
        if(EXRmap)
        {
            var bytes = totalMap.EncodeToEXR();
            File.WriteAllBytes(folderpath + "/maps/" + filepath + "_" + currentSession + "_map.exr", bytes);
            bytes = coverageMap.EncodeToEXR();
            File.WriteAllBytes(folderpath + "/maps/" + filepath + "_" + currentSession + "_coverage.exr", bytes);
            bytes = viewMap.EncodeToEXR();
            File.WriteAllBytes(folderpath + "/maps/" + filepath + "_" + currentSession + "_view.exr", bytes);
        }


        ComputeGems();
        summary.sessionSummaries[currentSession].physicalAccuracy = (float)correctPhysicalGems / (float)totalPhysicalGems;
        summary.sessionSummaries[currentSession].virtualAccuracy = (float)correctVirtualGems / (float)totalVirtualGems;
        summary.sessionSummaries[currentSession].floatingAccuracy = (float)correctFloatingGems / (float)totalFloatingGems;
        summary.sessionSummaries[currentSession].distanceTraveled = distanceTraveled;
        summary.sessionSummaries[currentSession].totalRotation = rotationMetric;
        summary.sessionSummaries[currentSession].totalSessionTime = totalSessionTime;

        summary.sessionSummaries[currentSession].visitedPhisycalGems = visitedPhisycalGems;
        summary.sessionSummaries[currentSession].visitedVirtualGems = visitedVirtualGems;
        summary.sessionSummaries[currentSession].visitedFloatingGems = visitedFloatingGems;


        summary.sessionSummaries[currentSession].wrongPhysicalGems = wrongPhysicalGems;
        summary.sessionSummaries[currentSession].wrongVirtualGems = wrongVirtualGems;
        summary.sessionSummaries[currentSession].wrongFloatingGems = wrongFloatingGems;


        summary.sessionSummaries[currentSession].repeatedPhysicalGems = repeatedPhysicalGems;
        summary.sessionSummaries[currentSession].repeatedVirtualGems = repeatedVirtualGems;
        summary.sessionSummaries[currentSession].repeatedFloatingGems = repeatedFloatingGems;


        if (totalAudio > 0)
        {
            summary.sessionSummaries[currentSession].audioAccuracy = (float)correctAudio / ((float)totalAudio);
            summary.sessionSummaries[currentSession].totalAudioCalls = totalAudio;
            summary.sessionSummaries[currentSession].correctAudioClicks = correctAudio;
        }
        

        startReplay = false;

        summary.SaveToJSON(folderpath, filepath);
        if (currentSession < studyObject.sessionRecordings.Length-1)
        {
            Debug.Log("load next scene...");
            NextScene();
        }
        else
        {
            if (writeJSON)
            {
                summary.SaveToJSON(folderpath,filepath);
                Debug.Log("writing json...");
                /*string json = JsonUtility.ToJson(studyData);
                File.WriteAllText(folderpath + "/extract/" + filepath + "_ex.json", json);*/
            }

            if (autoProceed)
            {
                Debug.Log("load next file...");
                LoadNextFile();
            }
            else
            {
                Debug.Break();
            }

        }
    }

    private void OnDrawGizmos()
    {
        showValidationGizmos = true;
        Gizmos.color = Color.cyan;
        Gizmos.matrix = head.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, 29f, 4f, 0.1f, 1.46f);
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.black;
        if (showValidationGizmos)
        {
            Gizmos.DrawRay(sf1.gazeOrigin, sf1.gazeDirection*5f);
        }


    }

    public Texture2D ScreenTexture(Camera _camera, RenderTexture rt, Shader shader, LayerMask mask)
    {
        var currentBG = _camera.backgroundColor;
        _camera.backgroundColor = new Color(0, 0, 0, 0);
        LayerMask currentMask = _camera.cullingMask;
        _camera.cullingMask = mask;

        _camera.targetTexture = rt;
        if (shader != null)
        {
            _camera.RenderWithShader(shader, "");
        }
        else
        {
            _camera.Render();
        }


        RenderTexture.active = rt;
        var sc = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false);
        sc.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
        sc.Apply();
        _camera.targetTexture = null;
        RenderTexture.active = null;
        _camera.cullingMask = currentMask;
        _camera.backgroundColor = currentBG;
        return sc;
    }

}
