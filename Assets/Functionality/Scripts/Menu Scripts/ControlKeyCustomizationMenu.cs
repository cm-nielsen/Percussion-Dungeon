using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControlKeyCustomizationMenu : MonoBehaviour
{
    public ControlKey target;
    private ControlKey.ControlUnit[] units;
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
        units = new ControlKey.ControlUnit[target.inputs.Count];
        target.inputs.CopyTo(units);
        GameObject g = new GameObject();
        Text t = (Text)g.AddComponent(typeof(Text));
        t.text = units[0].identifier;
        Instantiate(g, transform);
    }

    [Serializable]
    public class TextParameters
    {
        public Font font;
        public float size;
    }
}
