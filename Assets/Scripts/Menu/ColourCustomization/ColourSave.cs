using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ColourSave(string s)
    {
        string[] vals = s.Split('|');
        r = float.Parse(vals[0]);
        g = float.Parse(vals[1]);
        b = float.Parse(vals[2]);
    }

    public override string ToString()
    {
        return $"{r}|{g}|{b}";
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
