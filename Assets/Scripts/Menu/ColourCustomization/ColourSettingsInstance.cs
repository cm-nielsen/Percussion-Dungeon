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
