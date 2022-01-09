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
            GameController.SaveGameData();
    }
}

[System.Serializable]
public struct ColourSettings
{
    public ColourPair player, enemy, platform,
        background, health, objects, corpse;

    public ColourPair[] GetArray()
    {
        return new ColourPair[]{player, enemy, platform,
        background, health, objects, corpse};
    }

    [System.Serializable]
    public struct ColourPair
    {
        public SerializableColor main, mono;

        public ColourPair(Material m)
        {
            main = m.GetColor("_MainCol");
            mono = m.GetColor("_MonoCol");
        }

        public void Set(Material m)
        {
            main = m.GetColor("_MainCol");
            mono = m.GetColor("_MonoCol");
        }
    }
}


// From here:
// https://answers.unity.com/questions/772235/cannot-serialize-color.html
// needed to be able to save ColourSettings in a file
[System.Serializable]
public class SerializableColor
{

    public float[] colorStore = new float[4] { 1F, 1F, 1F, 1F };
    public Color Color
    {
        get { return new Color(colorStore[0], colorStore[1], colorStore[2], colorStore[3]); }
        set { colorStore = new float[4] { value.r, value.g, value.b, value.a }; }
    }

    //makes this class usable as Color, Color normalColor = mySerializableColor;
    public static implicit operator Color(SerializableColor instance)
    {
        return instance.Color;
    }

    //makes this class assignable by Color, SerializableColor myColor = Color.white;
    public static implicit operator SerializableColor(Color color)
    {
        return new SerializableColor { Color = color };
    }
}
