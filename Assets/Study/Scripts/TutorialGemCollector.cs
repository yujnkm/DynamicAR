using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialGemCollector : MonoBehaviour
{
    public bool giveFeedback = true;
    public List<GameObject> gems;
    public GameObject currentGem;
    private bool  gemInVicinity;
    public AudioClip[] correct;
    public AudioClip wrong;

    public float distance;
    private int correctGuesses;
    public int guessesToComplete = 10;
    public UnityEvent onComplete;
    public Transform cam;
    public bool singleGem;
    public bool destryoGems = true;
    private TutorialController controller;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        controller = GameObject.Find("TutorialController").GetComponent<TutorialController>();

        if (!singleGem)
        {
            NextGem();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        gemInVicinity = false;
        foreach (var gem in gems)
        {
            Vector3 dist = (gem.transform.position - cam.position);
            float angle = Vector3.Angle(cam.forward, dist);
            if (dist.magnitude < distance && angle<180)
            {    
                currentGem = gem;
                gemInVicinity = true;
                break;
            }
        }

        /*
        X->h
        A->g
        B->j
        Y->i
        */

        if (Input.GetKeyDown(KeyCode.H))
        {
            Guess(2);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Guess(1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Guess(0);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Guess(3);
        }
    }

    public void Guess(int index)
    {
        bool corr = false;
        if (gemInVicinity)
        {
            if(index == currentGem.GetComponent<Collectible>().type)
            {
                corr = true;
                int clipind = Random.Range(0, correct.Length);
                if (giveFeedback)
                {
                    GetComponent<AudioSource>().PlayOneShot(correct[clipind]);
                }
                
                correctGuesses++;
                if (correctGuesses < guessesToComplete)
                {
                    NextGem();
                }
                else
                {
                    onComplete.Invoke();
                }
            }
            else
            {
                if (giveFeedback)
                {
                    GetComponent<AudioSource>().PlayOneShot(wrong);
                }
            }
        }

        string line = "gem, " + Time.time.ToString("0.00") + ", " +corr+", " + index +", "+ currentGem.GetComponent<Collectible>().type;
        controller.WriteLine(line);

    }

    public void NextGem()
    {
        gems.Remove(currentGem);
        if (gems.Count > 0)
        {
            gems[0].SetActive(true);
        }
        if (destryoGems)
        {
            Destroy(currentGem);
        }
        
    }
}
