using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialGemIdentifier : MonoBehaviour
{
    public GameObject[] gemPrefabs;
    public FaceCamera fc;
    public GameObject currentGem;
    public AudioClip correct;
    public AudioClip wrong;
    private int correctGuesses;
    public int guessesToComplete = 10;
    public UnityEvent onComplete;

    // Start is called before the first frame update
    void Start()
    {
        NextGem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Guess(int index)
    {
        if(index == currentGem.GetComponent<Collectible>().type)
        {
            GetComponent<AudioSource>().PlayOneShot(correct);
            correctGuesses++;
            if(correctGuesses< guessesToComplete)
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
            GetComponent<AudioSource>().PlayOneShot(wrong);
        }
    }

    public void NextGem()
    {
        Destroy(currentGem);
        int rand = Random.Range(0, gemPrefabs.Length);
        currentGem = GameObject.Instantiate(gemPrefabs[rand], fc.transform);
        fc.Align();
    }
}
