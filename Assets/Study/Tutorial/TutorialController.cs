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
    public GameObject indicatorSystem;
    public GameObject vanishSystem;

    private Stack<int> previousIndexes = new Stack<int>();

    public void RestartTutorial()
    {
        Destroy(GameObject.Find("Dispatcher"));
        Destroy(GameObject.Find("WorldLockingUpdater"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    // Start is called before the first frame update
    void Start()
    {
        clips = GetComponentsInChildren<TutorialClip>(true);

        LoadClip(currentClip);

        sessionName = "tut-" + System.DateTime.Now.ToString("MM-dd-yyy_hh-mm")+".txt";
        WriteLine("type, time, value, guess, correct");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentClip == 1 || currentClip == 9)
        {
            if (!indicatorSystem.activeSelf)
            {
                indicatorSystem.SetActive(true);
                vanishSystem.SetActive(true);
            }
            Debug.Log(indicatorSystem.GetComponent<ParticleSystem>().isPlaying);
            if (!indicatorSystem.GetComponent<ParticleSystem>().isPlaying)
            {
                indicatorSystem.GetComponent<ParticleSystem>().Play();
            }
            if (!vanishSystem.GetComponent<ParticleSystem>().isPlaying)
            {
                vanishSystem.GetComponent<ParticleSystem>().Play();
            }
        }
        else
        {
            indicatorSystem.SetActive(false);
            vanishSystem.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Previous();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
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
            LoadClip(currentClip);
        }
    }
    public void JumpScene()
    {
        previousIndexes.Push(currentClip);
        currentClip = jumpIndex;
        LoadClip(currentClip);
    }

    public void Previous()
    {
        if (previousIndexes.Count > 0)
        {
            currentClip = previousIndexes.Pop();
        }
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
        text.text = index.ToString();
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
}


//left C
//right is D