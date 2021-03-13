using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public static List<System.Action> onBeat = new List<System.Action>(), offBeat = new List<System.Action>();
    public static bool beat = false;

    public float bpm;
    //public bool beat = false;

    private AudioSource aSource;

    private float lastTime, deltaTime, timer;

    void Awake()
    {
        OnSceneLoad();
        onBeat.Add(OnBeat);
        offBeat.Add(OffBeat);
        aSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoad;

        lastTime = deltaTime = timer = 0;
    }

    void Update()
    {
        deltaTime = aSource.time - lastTime;
        if (aSource.time < lastTime)
            deltaTime += aSource.clip.length;
        timer += deltaTime;
        lastTime = aSource.time;

        if(timer > 60 / bpm)
        {
            foreach (System.Action a in onBeat)
                a();
            timer -= 60 / bpm;
            beat = true;
        }else if (beat)
        {
            beat = false;
            foreach (System.Action a in offBeat)
                a();
        }
    }

    private void OnSceneLoad(Scene s = default(Scene), LoadSceneMode m = LoadSceneMode.Single)
    {
        onBeat.RemoveAll(x => x == null);
        offBeat.RemoveAll(x => x == null);
    }

    private void OnBeat()
    {
        //GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OffBeat()
    {
        //GetComponent<SpriteRenderer>().enabled = false;
    }
}
