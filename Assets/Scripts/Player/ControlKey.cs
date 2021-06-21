using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// <para/>A generalized keyboard control structure, created to abstract input collection.
/// <para/>Attach to an object, specify keycodes and give reference to movement script to use.
/// <para/>Input values can be accessed by [int], ["identifier"] or Get() Set() notation
/// </summary>
public class ControlKey : MonoBehaviour
{
    [Header("Inputs to manage")]
    public List <ControlUnit> inputs;

    /// <summary>
    /// Called every frame, collects input for every specified input
    /// </summary>
    private void Update()
    {
        foreach(ControlUnit unit in inputs)
        {
            unit.UpdateInputValue();
        }
    }

    /// <summary>
    /// (not case sensitive) Returns the input value associated with specified name, if it exists
    /// returns false if the name does not match any conatined in the input list
    /// </summary>
    /// <param name="name">the input identifier to search for, not case sensitive</param>
    /// <returns></returns>
    public bool Get(string name)
    {
        foreach (ControlUnit unit in inputs)
            if (unit.identifier.ToLower().Equals(name.ToLower()))
                return unit.GetVal();
        return false;
    }

    /// <summary>
    /// Returns the input value at the specified index, if it exists
    /// returns false if the index is invalid
    /// </summary>
    /// <param index="index">the index to access</param>
    /// <returns></returns>
    public bool Get(int index)
    {
        if(index >= inputs.Count)
            return false;
        return inputs[index].GetVal();
    }

    /// <summary>
    /// (not case sensitive) Returns the input set associated with specified name, if it exists
    /// returns null if the name does not match any conatined in the input list
    /// </summary>
    /// <param name="name">the input set to search for</param>
    /// <returns></returns>
    public ControlUnit GetUnit(string name)
    {
        foreach (ControlUnit unit in inputs)
            if (unit.identifier.ToLower().Equals(name.ToLower()))
                return unit;
        return null;
    }

    /// <summary>
    /// (not case sensitive) Returns the input value associated with specified name, if it exists
    /// returns false if the name does not match any conatined in the input list
    /// </summary>
    /// <param name="name">the input identifier to search for, not case sensitive
    /// <param name="val">the value to assign</param>
    public void Set(string name, bool val)
    {
        foreach (ControlUnit unit in inputs)
            if (unit.identifier.ToLower().Equals(name.ToLower()))
                unit.val = val;
    }

    /// <summary>
    /// sets the input value at the specified index, if it exists
    /// </summary>
    /// <param index="index">the index to access</param>
    /// <param name="val">the value to assign</param>
    public void Set(int index, bool val)
    {
        if (index >= inputs.Count)
            return;
        inputs[index].val = val;
    }

    /// <summary>
    /// Allows A ControlKey to be indexed with [] notation
    /// </summary>
    /// <param name="i">the index to access</param>
    /// <returns></returns>
    public bool this[int i]
    {
        get { return Get(i); }
        set { Set(i, value); }
    }

    /// <summary>
    /// Allows a ControlKey to be indexed with [] notation BUT WITH STRINGS
    /// </summary>
    /// <param name="s">the input to access</param>
    /// <returns></returns>
    public bool this[string s]
    {
        get { return Get(s); }
        set { Set(s, value); }
    }

    [Serializable]
    public class ControlUnit
    {
        [Header("[jump, left, shoot, etc.]")]
        [Space(-10)]
        [Header("Control name")]
        public string identifier;
        [Header("Current Value of input")]
        public bool val;
        [Header("How long to hold input [sec]")]
        public float holdTime;
        [Header("released in order to be triggered again")]
        [Space(-10)]
        [Header("True if the control must be")]
        public bool toggleInput;
        [Header("[w, downarrow, space, etc.]")]
        [Space(-10)]
        [Header("Key Codes")]
        public string[] keyCodes;
        [Header("[leftButton, etc.]")]
        [Space(-10)]
        [Header("Mouse Buttons")]
        public string[] mouseButtons;
        [Header("[buttonNorth, leftStick/right, etc.]")]
        [Space(-10)]
        [Header("Gamepad Buttons")]
        public string[] gamePadButtons;

        /// <summary>
        /// used to track the times at which a control was pressed, released (for input holding)
        /// </summary>
        private float timePressed = -100, timeReleased = 0;
        /// <summary>
        /// used for toggleInput feature
        /// </summary>
        private bool toggle = false;
        /// <summary>
        /// used to make timePressed only reflect the time the control was actuated, not held
        /// </summary>
        private bool timeToggle = true;

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="u">Control Unit to copy</param>
        public ControlUnit(ControlUnit u)
        {
            identifier = String.Copy(u.identifier);
            val = false;
            holdTime = u.holdTime;
            toggleInput = u.toggleInput;
            keyCodes = new string[u.keyCodes.Length];
            Array.Copy(u.keyCodes, keyCodes, keyCodes.Length);
            mouseButtons = new string[u.mouseButtons.Length];
            Array.Copy(u.mouseButtons, mouseButtons, mouseButtons.Length);
            gamePadButtons = new string[u.gamePadButtons.Length];
            Array.Copy(u.gamePadButtons, gamePadButtons, gamePadButtons.Length);
            timePressed = -100;
            timeReleased = 0;
            toggle = false;
            timeToggle = true;
        }

        public bool Equals(ControlUnit u)
        {
            if (identifier != u.identifier)
                return false;
            return keyCodes.SequenceEqual(u.keyCodes) &&
                mouseButtons.SequenceEqual(u.mouseButtons) &&
                gamePadButtons.SequenceEqual(u.gamePadButtons);
        }

        /// <summary>
        /// called by encapsulating class, updates val to reflect keyboard input
        /// </summary>
        public void UpdateInputValue()
        {
            Gamepad gamepad = Gamepad.current;
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            // runs a slight alteration for toggleInput feature, only reads new input if a false has been read
            if(toggleInput)
            {
                if (toggle)
                    toggle = GetSpecifiedInput(keyCodes, Keyboard.current) |
                    GetSpecifiedInput(mouseButtons, Mouse.current) |
                    GetSpecifiedInput(gamePadButtons, Gamepad.current);
                else
                    val = GetSpecifiedInput(keyCodes, Keyboard.current) |
                    GetSpecifiedInput(mouseButtons, Mouse.current) |
                    GetSpecifiedInput(gamePadButtons, Gamepad.current);
            }
            else
                // fetches inputs from devices
                val = GetSpecifiedInput(keyCodes, Keyboard.current) |
                    GetSpecifiedInput(mouseButtons, Mouse.current) |
                    GetSpecifiedInput(gamePadButtons, Gamepad.current);

            RunHoldTime();
        }

        /// <summary>
        /// CHecks to see if any of the potential inputs are pressed
        /// </summary>
        /// <param name="ar">inputs to check</param>
        /// <param name="input">device to check for inputs</param>
        /// <returns></returns>
        private bool GetSpecifiedInput(string[] ar, InputDevice input)
        {
            if (input == null)
                return false;
            foreach (string s in ar)
                if (input[s].IsPressed())
                    return true;

            return false;
        }

        /// <summary>
        /// Fetches the current value of input, consumes a true if toggleInput is active
        /// </summary>
        /// <returns>the value of the control</returns>
        public bool GetVal()
        {
            bool res = val || timePressed + holdTime > timeReleased;
            if (toggleInput && res)
            {
                val = false;
                toggle = true;
                timePressed = -100;
            }
            return res;
        }

        /// <summary>
        /// Executes logic behind holdTime feature
        /// </summary>
        private void RunHoldTime()
        {
            if (val && timeToggle)
            {
                timePressed = Time.time;
                timeToggle = false;
            }
            else if (!val)
            {
                timeReleased = Time.time;
                timeToggle = true;
            }
        }
    }
}
