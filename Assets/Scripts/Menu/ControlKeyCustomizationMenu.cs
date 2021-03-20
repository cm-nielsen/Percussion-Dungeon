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
    /// <summary>
    /// control key whihc the menu is modifying
    /// </summary>
    private ControlKey target;
    /// <summary>
    /// set of mapaple controls seperated by iunput device
    /// </summary>
    public PotentialControlNameSet controlSet;
    /// <summary>
    /// used to disable menu navigation when remapping inputs
    /// </summary>
    public ControlKey menuNavKey;

    /// <summary>
    /// control mapping text displays
    /// </summary>
    private Text[] displays;
    /// <summary>
    /// the current input being edited
    /// </summary>
    private ConUnit unit;
    /// <summary>
    /// used to disable menu navigation when remappinginput
    /// </summary>
    private BaseInputModule UIInput;
    /// <summary>
    /// which slot is being edited
    /// </summary>
    private int inputIndex = 0;

    private enum State { idle, listen, buttonSafeListen, noisePrevention}
    private State state = State.idle;
    // Start is called before the first frame update
    void Start()
    {
        displays = GetComponentsInChildren<Text>();
        UIInput = EventSystem.current.currentInputModule;
        target = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();
    }

    private void Update()
    {
        if (state == State.listen || state == State.buttonSafeListen)
        {
            StringPair input = ListenForInput();
            if (input != null)
            {
                RemapInput(input.val);
                state = State.noisePrevention;
                displays[inputIndex].GetComponent<BlinkOnInterval>().EndBlink();
            }
        }
        else if (state == State.noisePrevention && IsNoInput())
        {
            state = State.idle;
            UIInput.enabled = true;
            menuNavKey.enabled = true;
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
        int n = 0;
        foreach (string s in unit.keyCodes)
        {
            displays[n].text = controlSet.KeyboardRetrieve(s);
            if (++n >= displays.Length)
                return;
        }

        foreach (string s in unit.mouseButtons)
        {
            displays[n].text = controlSet.MouseRetrieve(s);
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
            if (n++ >= displays.Length)
                return;
        }

        while (n < displays.Length)
        {
            displays[n].text = "---";
            n++;
        }
    }

    public void BeginListening(int n)
    {
        if (unit == null)
            return;
        inputIndex = n;
        displays[inputIndex].GetComponent<BlinkOnInterval>().blink = true;
        state = State.listen;
        
        UIInput.enabled = false;
        menuNavKey.enabled = false;
    }
    /// <summary>
    /// find inpuy value to use for remapping
    /// </summary>
    /// <returns></returns>
    public StringPair ListenForInput()
    {
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
                state = State.buttonSafeListen;

            foreach(StringPair p in controlSet.gamepadMaps)
                if(Gamepad.current[p.val].IsPressed())
                    if(p.val != "buttonsouth" || state == State.buttonSafeListen)
                        return p;
        }
        return null;
    }
    /// <summary>
    /// checks to see if all input is silent, prevents unintentional menu navigation
    /// </summary>
    /// <returns></returns>
    public bool IsNoInput()
    {
        if (Gamepad.current == null)
        {
            foreach (StringPair p in controlSet.keyMaps)
                if (Keyboard.current[p.val].IsPressed())
                    return false;

            foreach (StringPair p in controlSet.mouseMaps)
                if (Mouse.current[p.val].IsPressed())
                    return false;
        }
        else
            foreach (StringPair p in controlSet.gamepadMaps)
                if (Gamepad.current[p.val].IsPressed())
                    return false;

        return true;
    }
    /// <summary>
    /// remaps the selected input slot with the given input value
    /// </summary>
    /// <param name="newValue"></param>
    public void RemapInput(string newValue)
    {
        if (Gamepad.current == null)
        {
            bool isKey = false;
            int keys = unit.keyCodes.Length;

            foreach (StringPair p in controlSet.keyMaps)
                if (p.val.ToLower() == newValue.ToLower())
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
    }
}

/// <summary>
/// set of acceptable input codes, seperated by input device
/// </summary>
[Serializable]
public class PotentialControlNameSet
{
    public StringPair[] keyMaps, mouseMaps, gamepadMaps;

    public string KeyboardRetrieve(string s)
    {
        foreach (StringPair p in keyMaps)
            if (p.val.ToLower() == s.ToLower())
                return p.name;
        return null;
    }

    public string MouseRetrieve(string s)
    {
        foreach (StringPair p in mouseMaps)
            if (p.val.ToLower() == s.ToLower())
                return p.name;
        return null;
    }

    public string GamepadRetrieve(string s)
    {
        foreach (StringPair p in gamepadMaps)
            if (p.val.ToLower() == s.ToLower())
                return p.name;
        return null;
    }
}

/// <summary>
/// Serializable string tuple
/// </summary>
[Serializable]
public class StringPair
{
    public string name;
    public string val;

    public override string ToString()
    {
        return "Display Name: " + name + " Value: " + val;
    }
} 