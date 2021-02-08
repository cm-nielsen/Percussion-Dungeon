using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static List<System.Action> onBeat, offBeat;

    public float bpm;
    public bool beat = false;

    private AudioSource aSource;

    private float lastTime, deltaTime, timer;
    private bool triggerOffBeat = false;

    void Awake()
    {
        onBeat = new List<System.Action>();
        offBeat = new List<System.Action>();
        onBeat.Add(OnBeat);
        offBeat.Add(OffBeat);
        aSource = GetComponent<AudioSource>();

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
            triggerOffBeat = true;
        }else if (triggerOffBeat)
        {
            triggerOffBeat = false;
            foreach (System.Action a in offBeat)
                a();
        }
    }

    private void OnBeat()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OffBeat()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
