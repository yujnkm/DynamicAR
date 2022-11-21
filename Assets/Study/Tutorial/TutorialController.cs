using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class TutorialController : MonoBehaviour
{
    public TutorialClip[] clips;
    public int currentClip = 0;
    public TMPro.TextMeshProUGUI text;
    public string sessionName;
    public int jumpIndex;
    public List<int> particleScenes;
    public List<int> arrowScenes;
    public GameObject spiralSystem;
    public float autoActivateTime;

    private Stack<int> previousIndexes = new Stack<int>();
    private int totalSceneDancers = 0;
    private int viewedDancers = 0;
    private float time = 0f;

    #region Singleton
    public static TutorialController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public void RestartTutorial()
    {
        Destroy(GameObject.Find("Dispatcher"));
        Destroy(GameObject.Find("WorldLockingUpdater"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Start()
    {
        clips = GetComponentsInChildren<TutorialClip>(true);

        ReadyNextScene();
        LoadClip(currentClip);

        sessionName = "tut-" + System.DateTime.Now.ToString("MM-dd-yyy_hh-mm")+".txt";
        WriteLine("type, time, value, guess, correct");

        //double security in case spiral is active in scene
        spiralSystem.SetActive(false);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Previous();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }

        //spiral activates once user views all dance sequences in scene
        if (time > autoActivateTime || viewedDancers == totalSceneDancers && totalSceneDancers != 0)
        {
            spiralSystem.SetActive(true);
        }
    }

    public void SetClip(TutorialClip targetclip)
    {
        clips = GetComponentsInChildren<TutorialClip>(true);
        for (var i=0; i < clips.Length;i++)
        {
            if(clips[i] == targetclip){
                currentClip = i;
            }
        }
        foreach (var clip in clips)
        {
            clip.gameObject.SetActive(false);
        }
        clips[currentClip].gameObject.SetActive(true);
    }

    public void Next()
    {
        if (currentClip < clips.Length - 1)
        {
            previousIndexes.Push(currentClip);
            currentClip++;
            if (currentClip == jumpIndex)
            {
                currentClip = clips.Length - 1;
            }
            ReadyNextScene();
            LoadClip(currentClip);
        }
    }
    public void JumpScene()
    {
        previousIndexes.Push(currentClip);
        currentClip = jumpIndex;
        ReadyNextScene();
        LoadClip(currentClip);
    }

    public void Previous()
    {
        if (previousIndexes.Count > 0)
        {
            currentClip = previousIndexes.Pop();
        }
        ReadyNextScene();
        LoadClip(currentClip);
    }

    public void LoadClip(int index)
    {
        foreach(var clip in clips)
        {
            clip.gameObject.SetActive(false);
        }
 
        clips[index].gameObject.SetActive(true);
        clips[index].Ready();
        clips[index].Play();

        currentClip = index;
        //text.text = index.ToString();
    }

    public void WriteLine(string line)
    {

        string path = Path.Combine(Application.persistentDataPath, sessionName);

        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(line);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(line);
            }
        }

    }

    public void incrementTotalDancers()
    {
        totalSceneDancers++;
    }

    public void incrementViewedDancers()
    {
        viewedDancers++;
    }

    private void resetDancerCount()
    {
        totalSceneDancers = 0;
        viewedDancers = 0;
    }

    private void ReadyNextScene()
    {
        //activates particles if current scene requires particles, and particles are not activated
        if (particleScenes.Contains(currentClip))
        {
            if (!IndicatorSystemController.Instance.gameObject.GetComponent<ParticleSystem>().isPlaying)
            {
                IndicatorSystemController.Instance.ActivateIndicators();
            }
        }
        else
        {
            IndicatorSystemController.Instance.StopIndicators();
        }

        //activates arrow if current scene require arrow, and arrow is not activated
        if (arrowScenes.Contains(currentClip))
        {
            //if arrow is already visible, ArrowFadeIn will not do anything
            ArrowController.Instance.ArrowFadeIn();
        }
        else
        {
            ArrowController.Instance.ArrowFadeOut();
        }

        resetDancerCount();
        spiralSystem.SetActive(false);
        time = 0f;
    }
}


//left C
//right is D