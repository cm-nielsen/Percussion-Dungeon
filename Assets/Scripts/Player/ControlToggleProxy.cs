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
