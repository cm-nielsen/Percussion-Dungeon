using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using ConUnit = ControlKey.ControlUnit;

public class ControlKeyCustomizationMenu : MonoBehaviour
{
    public ControlKey target;
    public TextObjectFactory headerFactory;

    private List<ConUnit> units;
    // Start is called before the first frame update
    void Start()
    {
        ConstructInterface();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ConstructInterface()
    {
        int len = target.inputs.Count;
        //units = new ConUnit[target.inputs.Count];
        //target.inputs.CopyTo(units);
        for(int i = 0; i < len; i++)
        {
            //Instantiate(headerParams.CreateObject((float)i / len, target.inputs[i].identifier), transform);
            headerFactory.CreateObject(transform, i / (len - 1f), target.inputs[i].identifier);
        }

        //foreach (ConUnit c in target.inputs)
        //    Instantiate(headerParams.CreateObject(0, c.identifier), transform);
    }

    [Serializable]
    public class TextObjectFactory
    {
        public Vector2 size, startPos, endPos;
        public Font font;
        public FontStyle fStyle;
        public int fSize;

        public void CreateObject(Transform tr, float val, string s)
        {
            GameObject g = new GameObject();
            g.name = s;
            g.transform.parent = tr;

            Text t = (Text)g.AddComponent<Text>();
            t.font = font;
            t.fontSize = fSize;
            t.fontStyle = fStyle;
            t.text = s;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform rect = g.GetComponent<RectTransform>();
            rect.localScale = size;
            rect.localPosition = Vector2.Lerp(startPos, endPos, val);
            rect.sizeDelta = size;
        }
    }
}
