using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ConUnit = ControlKey.ControlUnit;

public class ControlKeyCustomizationMenu : MonoBehaviour
{
    public ControlKey target;
    public PotentialControlNameSet controlSet;
    public ControlKey menuNavKey;

    //public StringPair[] keyMaps, mouseMaps, gamepadMaps;

    private Text[] displays;
    private string[] inputs;
    private ConUnit unit;
    private BaseInputModule UIInput;

    private int inputIndex = 0;
    private bool listen = false, buttonSafeguard = false;
    // Start is called before the first frame update
    void Start()
    {
        displays = GetComponentsInChildren<Text>();
        inputs = new string[displays.Length];
        UIInput = EventSystem.current.currentInputModule;
    }

    private void Update()
    {
        UIInput.enabled = !listen;
        menuNavKey.enabled = !listen;
        if (listen)
        {
            StringPair input = ListenForInput();
            //Debug.Log("Input found: " + input.val);
            if (input != null)
            {
                RemapInput(input.val);
                //menuNavKey.enabled = true;
            }
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
            displays[n].text = controlSet.KeyboardRetrieve(s);
            inputs[n] = s;
            if (++n >= displays.Length)
                return;
        }

        foreach (string s in unit.mouseButtons)
        {
            displays[n].text = controlSet.MouseRetrieve(s);
            inputs[n] = s;
            if (n++ >= displays.Length)
                return;
        }

        while(n < displays.Length)
        {
            displays[n].text = "---";
            n++;
        }
    }

    public void SetToGamepadInputs()
    {
        int n = 0;
        foreach (string s in unit.gamePadButtons)
        {
            displays[n].text = controlSet.GamepadRetrieve(s);
            inputs[n] = s;
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
        buttonSafeguard = false;
        //menuNavKey.enabled = false;
    }

    public StringPair ListenForInput()
    {
        //EventSystem.current.currentInputModule.enabled = false;

        if (Gamepad.current == null)
        {
            foreach (StringPair p in controlSet.keyMaps)
                if (Keyboard.current[p.val].IsPressed())
                    return p;

            foreach (StringPair p in controlSet.mouseMaps)
                if (Mouse.current[p.val].IsPressed())
                    return p;
        }
        else
        {
            if (!Gamepad.current["buttonsouth"].IsPressed())
                buttonSafeguard = true;

            foreach(StringPair p in controlSet.gamepadMaps)
                if(Gamepad.current[p.val].IsPressed())
                    if(p.val != "buttonsouth" || buttonSafeguard)
                        return p;
        }
        return null;
    }

    public void RemapInput(string newValue)
    {
        //Debug.Log("New Value: " + newValue + " at index " + inputIndex);
        if(Gamepad.current == null)
        {
            bool isKey = false;
            int keys = unit.keyCodes.Length;

            foreach (StringPair p in controlSet.keyMaps)
                if (p.val == newValue)
                    isKey = true;

            if (isKey)
            {
                if (inputIndex < keys)
                    unit.keyCodes[inputIndex] = newValue;
                else
                {
                    if (inputIndex < keys + unit.mouseButtons.Length)
                    {
                        Array.Resize(ref unit.keyCodes, keys + 1);
                        unit.keyCodes[keys] = newValue;

                        List<string> ls = unit.mouseButtons.ToList();
                        ls.RemoveAt(inputIndex - keys);
                        unit.mouseButtons = ls.ToArray();
                    }
                    else
                    {
                        Array.Resize(ref unit.keyCodes, keys + 1);
                        unit.keyCodes[keys] = newValue;
                    }
                }
            }
            else
            {
                if(inputIndex < keys)
                {
                    List<string> ls = unit.keyCodes.ToList();
                    ls.RemoveAt(inputIndex);
                    unit.keyCodes = ls.ToArray();

                    string[] newArr = new string[unit.mouseButtons.Length + 1];
                    newArr[0] = newValue;
                    Array.Copy(unit.mouseButtons, 0, newArr, 1, unit.mouseButtons.Length);
                    unit.mouseButtons = newArr;
                }
                else if (inputIndex < keys + unit.mouseButtons.Length)
                    unit.mouseButtons[inputIndex - keys] = newValue;
                else
                {
                    Array.Resize(ref unit.mouseButtons, unit.mouseButtons.Length + 1);
                    unit.mouseButtons[unit.mouseButtons.Length - 1] = newValue;
                }
            }
            SetToKeyboardInputs();
        }
        else
        {
            if (inputIndex < unit.gamePadButtons.Length)
                unit.gamePadButtons[inputIndex] = newValue;
            else
            {
                string[] newArr = new string[unit.gamePadButtons.Length + 1];
                unit.gamePadButtons.CopyTo(newArr, 0);
                newArr[newArr.Length - 1] = newValue;
                unit.gamePadButtons = newArr;
            }
            SetToGamepadInputs();
        }
        listen = false;
    }
}

[Serializable]
public class PotentialControlNameSet
{
    public StringPair[] keyMaps, mouseMaps, gamepadMaps;

    public string KeyboardRetrieve(string s)
    {
        foreach (StringPair p in keyMaps)
            if (p.val == s)
                return p.name;
        return null;
    }

    public string MouseRetrieve(string s)
    {
        foreach (StringPair p in mouseMaps)
            if (p.val == s)
                return p.name;
        return null;
    }

    public string GamepadRetrieve(string s)
    {
        foreach (StringPair p in gamepadMaps)
            if (p.val == s)
                return p.name;
        return null;
    }
}

[Serializable]
public class StringPair
{
    public string name;
    public string val;
} 