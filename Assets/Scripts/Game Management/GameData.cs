using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct GameData
{
    public static int healthUpgrades,
        castas;

    public static float masterVol, musicVol, sfxVol;

    public static WeaponUnlocks unlocks, current;

    public static VisualEffectSettings vfxSettings;

    public static WeaponExperience experience;

    public static List<ControlKey.ControlUnit> pControls;

    public static ColourSettings colours;

    public static void Load(GameDataInstance i)
    {
        healthUpgrades = i.healthUpgrades;
        castas = i.castas;
        masterVol = i.masterVol;
        musicVol = i.musicVol;
        sfxVol = i.sfxVol;
        unlocks = i.unlocks;
        current = i.current;
        vfxSettings = i.vfxSettings;
        experience = i.experience;
        pControls = i.pControls;
        colours = i.colours;
    }
}

[System.Serializable]
public struct GameDataInstance
{
    public int healthUpgrades, castas;
    public float masterVol, musicVol, sfxVol;
    public WeaponUnlocks unlocks, current;
    public VisualEffectSettings vfxSettings;
    public WeaponExperience experience;
    public List<ControlKey.ControlUnit> pControls;
    public ColourSettings colours;

    public GameDataInstance(bool b = false)
    {
        healthUpgrades = GameData.healthUpgrades;
        castas = GameData.castas;
        masterVol = GameData.masterVol;
        musicVol = GameData.musicVol;
        sfxVol = GameData.sfxVol;
        unlocks = GameData.unlocks;
        current = GameData.current;
        vfxSettings = GameData.vfxSettings;
        experience = GameData.experience;
        pControls = GameData.pControls;
        colours = GameData.colours;
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetInt("health upgrades", healthUpgrades);
        PlayerPrefs.SetInt("castas", castas);
        PlayerPrefs.SetFloat("master volume", masterVol);
        PlayerPrefs.SetFloat("music volume", musicVol);
        PlayerPrefs.SetFloat("sfx volume", sfxVol);
        PlayerPrefs.SetInt("unlocks", (int)unlocks);
        PlayerPrefs.SetInt("current", (int)current);
        PlayerPrefs.SetString("vfx settings", vfxSettings.ToString());
        PlayerPrefs.SetString("weapon experience", experience.ToString());
        PlayerPrefs.SetString("controls", ControlsToString());
        PlayerPrefs.SetString("colours", colours.ToString());
    }

    public void ReadFromPrefs()
    {
        healthUpgrades = PlayerPrefs.GetInt("health upgrades", 0);
        castas = PlayerPrefs.GetInt("castas", 0);
        masterVol = PlayerPrefs.GetFloat("master volume", .5f);
        musicVol = PlayerPrefs.GetFloat("music volume", .5f);
        sfxVol = PlayerPrefs.GetFloat("sfx volume", .5f);
        unlocks = (WeaponUnlocks)PlayerPrefs.GetInt("unlocks", 0);
        current = (WeaponUnlocks)PlayerPrefs.GetInt("current", 0);
        vfxSettings = new VisualEffectSettings(PlayerPrefs.GetString("vfx settings"));
        experience = new WeaponExperience(PlayerPrefs.GetString("weapon experience"));
        pControls = ParseControls(PlayerPrefs.GetString("controls"));
        colours = new ColourSettings(PlayerPrefs.GetString("colours"));
    }

    private string ControlsToString()
    {
        return string.Join("\\", pControls);
    }

    private List<ControlKey.ControlUnit> ParseControls(string s)
    {
        return s.Split('\\').ToList().ConvertAll(ControlKey.ControlUnit.Parse);
    }
}
