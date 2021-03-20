using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkOnInterval : MonoBehaviour
{
    public Behaviour componentToBlink;
    public float interval;
    public bool blink = false;

    private float count = 0;

    // Update is called once per frame
    private void Update()
    {
        count += Time.unscaledDeltaTime;
        if (count > interval && blink)
        {
            count = 0;
            componentToBlink.enabled = !componentToBlink.enabled;
        }
    }

    public void EndBlink()
    {
        count = 0;
        componentToBlink.enabled = true;
        blink = false;
    }
}
