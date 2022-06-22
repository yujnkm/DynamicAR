using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{

    public const int NUM_BLOCKS = 2;
    public const int NUM_TRIALS = 3;
    public const int NUM_SESSIONS = NUM_BLOCKS * NUM_TRIALS;
    public const int NUM_BACKUP = 4;
    public const int NUM_TOTAL = NUM_SESSIONS + NUM_BACKUP;
    public const int NUM_WORDS = 5;
    public const int NUM_AUDIO = 6;
    public const int NUM_GEMS = 24;
    public const int NUM_LAYOUTS = 8;
    public const float GEM_DIST = 4.0f;
    public const int GEM_ANGLE = 40;
/*    public const int NUM_ANCHORS = 1;
    public const int NUM_ANCHORS_TEST = 10;*/
    public const float CLICK_DURATION = 3f;
    public static string[] CALL_WORDS = { "alpha", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel", "india", "juliet" };
    public static float WORD_DELAY = 0.5f;
    public static float INITIAL_DELAY = 10.0f;
    public const float TICK_RATE = 6f;
    public const bool FIRST_PERSON_PLAYBACK = false;
    public const int FRAME_RATE = 60;
    public const string PROJECT_PATH = "https://aug-cog-walking-default-rtdb.firebaseio.com/users";

}
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
public class Constants : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
