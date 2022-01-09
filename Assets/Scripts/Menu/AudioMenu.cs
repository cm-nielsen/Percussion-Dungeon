using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour//, RequiresInitialSetup
{
    public Slider master, sfx, music;

    public bool SaveEnabled = false;

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

        SaveEnabled = true;
    }

    public void SetMasterVolume(float num)
    {
        AudioListener.volume = num;
        if (SaveEnabled)
        {
            GameData.masterVol = num;
            GameController.SaveGameData();
        }
    }

    public void SetEffectVolume(float num)
    {
        AudioClipPlayer.settings.volume = num;
        foreach (LoopingAudioClipPlayer player in
            FindObjectsOfType<LoopingAudioClipPlayer>())
            player.UpdateParameters();
        if (SaveEnabled)
        {
            GameData.sfxVol = num;
            GameController.SaveGameData();
        }
    }

    public void SetMusicVolume(float num)
    {
        GameObject.FindObjectOfType<Music>().GetComponent<AudioSource>().volume = num;
        if (SaveEnabled)
        {
            GameData.musicVol = num;
            GameController.SaveGameData();
        }
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
