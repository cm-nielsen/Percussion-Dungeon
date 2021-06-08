using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class ControlSteadyProxy
{
    public bool val
    {
        get
        {
            con.UpdateInputValue();
            return con.val;
        }
    }

    public ControlKey.ControlUnit con;

    public void Setup(ControlKey k, string s)
    {
        con = k.GetUnit(s);
        con = new ControlKey.ControlUnit(con);
        con.toggleInput = false;
    }
}