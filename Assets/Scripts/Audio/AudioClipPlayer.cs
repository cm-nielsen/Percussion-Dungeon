using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour
{
    public static AudioSourceSettings settings;

    private List<AudioSource> sources = new List<AudioSource>();

    private void Update()
    {
        List<AudioSource> toDestroy = new List<AudioSource>();

        foreach( AudioSource s in sources)
            if (!s.isPlaying)
                toDestroy.Add(s);

        foreach(AudioSource s in toDestroy)
        {
            Destroy(s);
            sources.Remove(s);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        ApplyParameters(s);

        s.clip = clip;
        s.Play();
        sources.Add(s);
    }

    public static void ApplyParameters(AudioSource s)
    {
        s.volume = settings.volume;
        s.pitch = settings.pitch;
        s.dopplerLevel = settings.doppler;
        s.minDistance = settings.minDist;
        s.maxDistance = settings.maxDist;
        s.spatialBlend = settings.spatialBlend;
        s.rolloffMode = settings.rollMode;
    }
}

[System.Serializable]
public struct AudioSourceSettings
{
    [Range(0, 1)]
    public float spatialBlend, volume;
    public AudioRolloffMode rollMode;
    public float pitch, doppler, spread, minDist, maxDist;

    public void Apply(AudioSourceSettings s)
    {
        float vol = volume;
        this = s;
        volume = vol;
    }
}
