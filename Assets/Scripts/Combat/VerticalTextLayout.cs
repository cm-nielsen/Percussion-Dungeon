using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalTextLayout : MonoBehaviour
{
    public float spacing = 0.5f;
    public float scale = .25f;
    public int maxCount = 6;
    public bool extendUpwards;
    public Font font;
    public int fontSize;
    public FontStyle fStyle;
    public Material material;

    private List<Text> text = new List<Text>();
    private Vector2 cursor = Vector2.zero;

    //private SpriteRenderer background;

    // Start is called before the first frame update
    void Start()
    {
        //background = GetComponentInChildren<SpriteRenderer>();
        //background.size = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //Color c = material.GetColor("_MainCol");
        //c.a = .25f;
        //background.color = c;
    }

    public void Append(string s)
    {
        if (text.Count == maxCount)
            RemoveOldest();

        GameObject inst = new GameObject("verticalTextLayout child");
        inst.transform.parent = transform;
        inst.transform.localPosition = cursor;
        inst.transform.localScale = new Vector2(scale, scale);
        if (extendUpwards)
            cursor += Vector2.up * spacing;
        else
            cursor += Vector2.down * spacing;
        Text t = inst.AddComponent<Text>();
        t.text = s;
        t.font = font;
        t.fontSize = fontSize;
        t.fontStyle = fStyle;
        t.alignment = TextAnchor.UpperRight;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.alignByGeometry = true;
        t.material = material;
        inst.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        text.Add(t);
        //background.size = new Vector2(1, Mathf.Abs(cursor.y));
    }

    public void ModifyNewest(string s)
    {
        if (text.Count == 0)
            Append(s);
        text[text.Count - 1].text = s;
    }

    private void RemoveOldest()
    {
        Destroy(text[0]);
        text.RemoveAt(0);

        Vector2 offset = Vector2.up * spacing;
        if (extendUpwards)
            offset.y *= -1;
        foreach (Text t in text)
            t.transform.Translate(offset);
        cursor += offset;
    }

    public void Clear()
    {
        foreach (Text t in text)
            Destroy(t.gameObject);
        text.Clear();
        cursor = Vector2.zero;
        //background.size = Vector2.zero;
    }
}
