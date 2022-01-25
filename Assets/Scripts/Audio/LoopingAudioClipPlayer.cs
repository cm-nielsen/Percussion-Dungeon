using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingAudioClipPlayer : MonoBehaviour
{
    public AudioClip clip;
    [Range(0, 1f)]
    public float volume = 1;
    public bool playOnAwake = true;

    private AudioSource source;
    private float settingsVolume;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        UpdateParameters();
        if (playOnAwake)
            source.Play();
    }

    private void Update()
    {
        source.volume = settingsVolume * volume;
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
        settingsVolume = source.volume;
        source.volume *= volume;
        source.clip = clip;
        source.loop = true;
    }
}
