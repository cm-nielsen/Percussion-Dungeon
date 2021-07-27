using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReverseColourMaterial : MonoBehaviour
{
    public Material referenceMaterial, modifiedMaterial;
    public string propertyName;

    void Update()
    {
        Color c = ReverseColour(referenceMaterial.GetColor(propertyName));
        modifiedMaterial.SetColor(propertyName, c);
    }

    public static Color ReverseColour(Color c)
    {
        float[] ar = { c.r, c.g, c.b };

        int minIndex = System.Array.IndexOf(ar, ar.Min());
        int maxIndex = System.Array.IndexOf(ar, ar.Max());

        float x = ar[minIndex];
        ar[minIndex] = ar[maxIndex];
        ar[maxIndex] = x;

        Color reversed = new Color(ar[0], ar[1], ar[2]);
        if (ColourDifference(c, reversed) < 0.4f)
            return new Color(1 - c.r, 1 - c.g, 1 - c.b);

        return new Color(ar[0], ar[1], ar[2]);
    }

    private static float ColourDifference(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
    }
}
