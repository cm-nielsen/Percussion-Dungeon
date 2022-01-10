using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ColourSettings(string s)
    {
        string[] vals = s.Split(',');
        int i = 0;

        player = new ColourSavePair(vals[i++]);
        enemy = new ColourSavePair(vals[i++]);
        platform = new ColourSavePair(vals[i++]);
        background = new ColourSavePair(vals[i++]);
        health = new ColourSavePair(vals[i++]);
        objects = new ColourSavePair(vals[i++]);
        corpse = new ColourSavePair(vals[i++]);
    }

    public override string ToString()
    {
        ColourSavePair[] pairs = GetArray();
        string s = "" + pairs[0];

        for (int i = 1; i < pairs.Length; i++)
            s += "," + pairs[i];

        return s;
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

        public ColourSavePair(string s)
        {
            string[] vals = s.Split('&');
            main = new ColourSave(vals[0]);
            mono = new ColourSave(vals[1]);
        }

        public override string ToString()
        {
            return $"{main}&{mono}";
        }

        public void Set(Material m)
        {
            main = m.GetColor("_MainCol");
            mono = m.GetColor("_MonoCol");
        }

        //public static implicit operator ColourSavePair(Material m)
        //{
        //    return new ColourSavePair(m);
        //}
    }
}
