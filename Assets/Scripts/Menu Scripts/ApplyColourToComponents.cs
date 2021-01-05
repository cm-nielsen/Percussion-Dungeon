using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyColourToComponents : MonoBehaviour
{
    public enum GetColourFrom { sprite, image, text, none}
    public GetColourFrom source = GetColourFrom.none;
    public Color colour;

    public SpriteRenderer[] sprites;
    public Image[] images;
    public Text[] texts;

    private void Start()
    {
        switch (source)
        {
            case GetColourFrom.sprite:
                colour = GetComponent<SpriteRenderer>().color;
                break;

            case GetColourFrom.image:
                colour = GetComponent<Image>().color;
                break;

            case GetColourFrom.text:
                colour = GetComponent<Text>().color;
                break;
        }
    }

    public void Apply()
    {
        foreach (SpriteRenderer s in sprites)
            s.color = colour;

        foreach (Image i in images)
            i.color = colour;

        foreach (Text t in texts)
            t.color = colour;
    }
}
