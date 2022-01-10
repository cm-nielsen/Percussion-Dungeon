using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows to the use of a regular ControlUnit as a toggled input
/// </summary>
public class ControlToggleProxy
{
    public bool val
    {
        get
        {
            if (value)
            {
                value = false;
                toggle = true;
                return true;
            }
            return false;
        }
    }
    private bool toggle = true, value = false;

    private ControlKey key;
    private string control;

    public void Setup(ControlKey k, string s)
    {
        key = k;
        control = s;
    }

    public void Update()
    {
        if (toggle)
            toggle = key[control];
        else
            value = key[control];
    }
}

/// <summary>
/// Allows for the use of a toggled ControlUnit as untoggled
/// </summary>
[System.Serializable]
public class ControlSteadyProxy
{
    public bool val
    {
        get
        {
            if (con == null || Time.timeScale == 0)
                return false;

            if (!con.Equals(basis))
                Setup(key, basis.identifier);
            con.UpdateInputValue();
            return con.val;
        }
    }

    public ControlKey.ControlUnit con;
    private ControlKey key;
    private ControlKey.ControlUnit basis;

    public void Setup(ControlKey k, string s)
    {
        key = k;
        basis = k.GetUnit(s);
        if (basis == null)
            Debug.Log("Steady Control Proxy could not be created: " + s);
        con = new ControlKey.ControlUnit(basis);
        con.toggleInput = false;
    }
}