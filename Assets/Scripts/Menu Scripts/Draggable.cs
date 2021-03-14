using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
    public Vector2 size = Vector2.zero;
    public float stickSpeed = 0.2f;

    private Vector2 oldMousePos, posLim = Vector2.zero, negLim = Vector2.zero;
    private Vector2 mousePos {
        get { return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); }
    }
    private bool active = false;
    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.IsPressed())
        {
            if (active)
            {
                Move(mousePos - oldMousePos);
                oldMousePos = mousePos;
            }
            else
            {
                active = true;
                oldMousePos = mousePos;
            }
        }
        else
            active = false;

        if (Gamepad.current != null)
        {
            Move(Gamepad.current.rightStick.ReadValue() * stickSpeed);
        }
    }

    private void Move(Vector2 v)
    {
        Vector2 pos = transform.localPosition;
        pos += v;

        float f = posLim.x + size.x;
        if (pos.x > f)
            pos.x = f;
        f = negLim.x - size.x;
        if (pos.x < f)
            pos.x = f;
        f = posLim.y + size.y;
        if (pos.y > f)
            pos.y = f;
        f = negLim.y - size.y;
        if (pos.y < f)
            pos.y = f;

        transform.localPosition = pos;
    }

    private void OnDisable()
    {
        active = false;
    }

    public void ExpandLimits(Vector2 v)
    {
        if (v.x > posLim.x)
            posLim.x = v.x;
        else if (v.x < negLim.x)
            negLim.x = v.x;
        if (v.y > posLim.y)
            posLim.y = v.y;
        else if (v.y < negLim.y)
            negLim.y = v.y;
    }
}
