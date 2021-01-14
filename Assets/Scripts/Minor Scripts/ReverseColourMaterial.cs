using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReverseColourMaterial : MonoBehaviour
{
    public Material referenceMaterial, modifiedMaterial;
    public string propertyName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Color c = ReverseColour(referenceMaterial.GetColor(propertyName));
        modifiedMaterial.SetColor(propertyName, c);
    }

    private Color ReverseColour(Color c)
    {
        float[] ar = { c.r, c.g, c.b };

        int minIndex = System.Array.IndexOf(ar, ar.Min());
        int maxIndex = System.Array.IndexOf(ar, ar.Max());

        float x = ar[minIndex];
        ar[minIndex] = ar[maxIndex];
        ar[maxIndex] = x;

        return new Color(ar[0], ar[1], ar[2]);
    }
}
