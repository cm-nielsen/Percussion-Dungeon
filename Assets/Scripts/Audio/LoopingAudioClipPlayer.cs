using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingAudioClipPlayer : MonoBehaviour
{
    public AudioClip clip;
    public bool playOnAwake = true;

    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        UpdateParameters();
        if (playOnAwake)
            source.Play();
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void UpdateParameters()
    {
        AudioClipPlayer.ApplyParameters(source);
        source.clip = clip;
        source.loop = true;
    }
}
