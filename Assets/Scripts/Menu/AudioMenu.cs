using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour//, RequiresInitialSetup
{
    public Slider master, sfx, music;

    public void OnEnable()
    {
        float num = GameData.masterVol;
        master.value = num;
        AudioListener.volume = num;

        num = GameData.sfxVol;
        sfx.value = num;
        AudioClipPlayer.settings.volume = num;

        num = GameData.musicVol;
        music.value = num;
        GameObject.FindObjectOfType<Music>().GetComponent<AudioSource>().volume = num;
    }

    public void SetMasterVolume(float num)
    {
        AudioListener.volume = num;
        GameData.masterVol = num;
        GameController.SaveGameData();
    }

    public void SetEffectVolume(float num)
    {
        AudioClipPlayer.settings.volume = num;
        GameData.sfxVol = num;
        GameController.SaveGameData();
    }

    public void SetMusicVolume(float num)
    {
        GameObject.FindObjectOfType<Music>().GetComponent<AudioSource>().volume = num;
        GameData.musicVol = num;
        GameController.SaveGameData();
    }

    public static void SetAll()
    {
        float num = GameData.masterVol;
        AudioListener.volume = num;

        num = GameData.sfxVol;
        AudioClipPlayer.settings.volume = num;

        num = GameData.musicVol;
        GameObject.FindObjectOfType<Music>().GetComponent<AudioSource>().volume = num;
    }
}
