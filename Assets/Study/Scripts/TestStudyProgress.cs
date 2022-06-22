using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestStudyProgress : MonoBehaviour
{
    public Dropdown dd;
    public StudyParser parser;
    public int currentSession;
    public int currentUser;

    public Text debugtext;
    // Start is called before the first frame update
    void Awake()
    {
        parser = GetComponent<StudyParser>();
        parser.OnStudyLoaded.AddListener(PropagateList);
    }


    void PropagateList()
    {
        List<string> options = new List<string>();
        for(int i=0;i< parser.alltrials.trials.Length; i++)
        {
            options.Add(parser.alltrials.trials[i].userid);
        }

        dd.AddOptions(options);
    }

    public void LoadUser(int ind)
    {
        currentUser = ind;
        GoTo(currentSession);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Previous();
        }
    }

    public void Next()
    {
        if (currentSession < parser.alltrials.trials[currentUser].sessions.Length - 1)
        {
            currentSession = currentSession + 1;
            GoTo(currentSession);
        }
        
    }

    public void Previous()
    {
        if (currentSession > 0 )
        {
            currentSession = currentSession - 1;
            GoTo(currentSession);
        }
        
    }

    public void GoTo(int index)
    {
        string userid = parser.alltrials.trials[currentUser].userid;
        int call_word = parser.alltrials.trials[currentUser].sessions[currentSession].call_word;
        int word_track = parser.alltrials.trials[currentUser].sessions[currentSession].word_track;
        int delay_track = parser.alltrials.trials[currentUser].sessions[currentSession].delay_track;
        int map = parser.alltrials.trials[currentUser].sessions[currentSession].map;
        bool target_present = parser.alltrials.trials[currentUser].sessions[currentSession].target_present;
        int target_combination = parser.alltrials.trials[currentUser].sessions[currentSession].target_combination;
        bool audio_present= parser.alltrials.trials[currentUser].sessions[currentSession].audio_present;


        string sesstring = $"Hello, {userid}! " + "\n" +
            $"This is session {currentSession} of { parser.alltrials.trials[currentUser].sessions.Length - 1} for this trial\n" +
            $"call word is {call_word}" + "\n" +
            $"word track is {word_track}" + "\n" +
            $"delay track is {delay_track}" + "\n" +
            $"map is {map}" + "\n" +
            $"target present is {target_present}" + "\n" +
            $"target combination is {target_combination}" + "\n" +
            $"audio present is {audio_present}" + "\n";

        debugtext.text = sesstring;
    }
}
