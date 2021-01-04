using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

using ConUnit = ControlKey.ControlUnit;

public class ControlKeyCustomizationMenu : MonoBehaviour
{
    public ControlKey target;
    public int please;
    public PotentialControlNameSet controlSet;

    //public StringPair[] keyMaps, mouseMaps, gamepadMaps;

    private Text[] displays;
    private ConUnit unit;
    private int inputIndex = 0;
    private bool listen = false;
    // Start is called before the first frame update
    void Start()
    {
        displays = GetComponentsInChildren<Text>();
    }

    private void Update()
    {
        if (listen)
        {
            string input = ListenForInput();
            Debug.Log("Input found: " + input);
            if (input != null && input != "---")
                RemapInput(input);
        }
    }

    public void SetActiveUnit(string identifier)
    {
        unit = target.GetUnit(identifier);

        if (Gamepad.current == null)
            SetToKeyboardInputs();
        else
            SetToGamepadInputs();
    }

    public void SetToKeyboardInputs()
    {
        //Debug.Log(displays.Length);
        int n = 0;
        foreach (string s in unit.keyCodes)
        {
            displays[n].text = s;
            if (++n >= displays.Length)
                return;
        }

        foreach (string s in unit.mouseButtons)
        {
            displays[n].text = s;
            if (n++ >= displays.Length)
                return;
        }
    }

    public void SetToGamepadInputs()
    {
        int n = 0;
        foreach (string s in unit.gamePadButtons)
        {
            displays[n].text = s;
            if (n++ >= displays.Length)
                return;
        }
    }

    public void BeginListening(int n)
    {
        if (unit == null)
            return;
        inputIndex = n;
        listen = true;
    }

    public string ListenForInput()
    {
        if (Gamepad.current == null)
        {
            foreach (InputControl ic in Keyboard.current.allKeys)
                if (ic.IsPressed())
                {
                    string s = controlSet.RetrieveDisplayName(ic.name);
                    if (s != null)
                    {
                        listen = false;
                        return ic.name;
                    }
                }

            foreach (InputControl ic in Mouse.current.allControls)
                if (ic.IsPressed())
                {
                    string s = controlSet.RetrieveDisplayName(ic.name);
                    if (s != null)
                    {
                        listen = false;
                        return ic.name;
                    }
                }
        }
        else
        {
            foreach (InputControl ic in Gamepad.current.allControls)
                if (ic.IsPressed())
                {
                    string s = controlSet.RetrieveDisplayName(ic.name);
                    if (s != null)
                    {
                        listen = false;
                        return ic.name;
                    }
                }
        }
        return "---";
    }

    public void RemapInput(string newValue)
    {
        int n = 0;
        Debug.Log("New Value: " + newValue + " at index " + inputIndex);
        if(Gamepad.current == null)
        {
            foreach (string s in unit.keyCodes)
            {
                if (n++ == inputIndex)
                {
                    //newValue.CopyTo(0, s, 0, newValue.Length);
                    s.Replace(s, newValue);
                    Debug.Log(s);
                }
            }

            foreach (string s in unit.mouseButtons)
            {
                if (n++ == inputIndex)
                    s.Replace(s, newValue);
            }
            SetToKeyboardInputs();
        }
        else
        {
            foreach (string s in unit.gamePadButtons)
            {
                if (n++ == inputIndex)
                    s.Replace(s, newValue);
            }
            SetToGamepadInputs();
        }
    }
}

[Serializable]
public class PotentialControlNameSet
{
    public StringPair[] keyMaps, mouseMaps, gamepadMaps;

    public string RetrieveDisplayName(string s)
    {
        foreach (StringPair p in keyMaps)
            if (p.val == s)
                return p.name;

        foreach (StringPair p in mouseMaps)
            if (p.val == s)
                return p.name;

        foreach (StringPair p in gamepadMaps)
            if (p.val == s)
                return p.name;
        return null;
    }
}

[Serializable]
public struct StringPair
{
    public string name;
    public string val;
} 