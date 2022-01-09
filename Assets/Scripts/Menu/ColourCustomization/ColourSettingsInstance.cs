using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourSettingsInstance : MonoBehaviour
{
    public static ColourSettings colourSettings;
    public ColourSettings debug;

    public Material player, enemy, platform,
        background, health, objects, corpse;

    public ColourPreset defaultPreset;

    public ColourSettings ApplyDefault()
    {
        Material[] mats = new Material[]
            {background, enemy, player,
            objects, platform, health, corpse };

        int materialIndex = 0;
        foreach(ColourPair p in defaultPreset)
        {
            mats[materialIndex].SetColor("_MainCol", p.main);
            mats[materialIndex].SetColor("_MonoCol", p.mono);
            materialIndex++;
        }
        UpdateColours(false);
        return colourSettings;
    }

    public void Start()
    {
        colourSettings = GameData.colours;

        Material[] mats = new Material[]{player, enemy,
            platform, background, health, objects, corpse};
        var colourPairs = colourSettings.GetArray();

        for(int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor("_MainCol", colourPairs[i].main);
            mats[i].SetColor("_MonoCol", colourPairs[i].mono);
        }
        debug = colourSettings;
    }

    public void UpdateColours(bool save = true)
    {
        colourSettings.player.Set(player);
        colourSettings.enemy.Set(enemy);
        colourSettings.platform.Set(platform);
        colourSettings.background.Set(background);
        colourSettings.health.Set(health);
        colourSettings.objects.Set(objects);
        colourSettings.corpse.Set(corpse);
        if (save)
        {
            GameData.colours = colourSettings;
            GameController.SaveGameData();
        }

        debug = colourSettings;
    }
}

[System.Serializable]
public struct ColourSettings
{
    public ColourSavePair player, enemy, platform,
        background, health, objects, corpse;

    public ColourSavePair[] GetArray()
    {
        return new ColourSavePair[]{player, enemy, platform,
        background, health, objects, corpse};
    }

    [System.Serializable]
    public struct ColourSavePair
    {
        public ColourSave main, mono;

        public ColourSavePair(Material m)
        {
            main = m.GetColor("_MainCol");
            mono = m.GetColor("_MonoCol");
        }

        public void Set(Material m)
        {
            main = m.GetColor("_MainCol");
            mono = m.GetColor("_MonoCol");
        }

        public static implicit operator ColourSavePair(Material m)
        {
            return new ColourSavePair(m);
        }
    }
}

[System.Serializable]
public struct ColourSave
{
    public float r, g, b;

    public ColourSave(Color color)
    {
        r = color.r;
        g = color.g;
        b = color.b;
    }

    public static implicit operator ColourSave(Color color)
    {
        return new ColourSave(color);
    }

    public static implicit operator Color(ColourSave color)
    {
        return new Color(color.r, color.g, color.b);
    }
}
