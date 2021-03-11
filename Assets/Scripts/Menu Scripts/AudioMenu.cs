using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenu : MonoBehaviour
{
    public void SetMasterVolume(float num)
    {
        AudioListener.volume = num;
    }

    public void SetEffectVolume(float num)
    {
        AudioClipPlayer.settings.volume = num;
    }

    public void SetMusicVolume(float num)
    {
        GameObject.FindObjectOfType<Music>().GetComponent<AudioSource>().volume = num;
    }
}
