using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOverrideArea : MonoBehaviour
{
    public bool triggerWithCollision = true, resetWithCollision = true;
    public Vector2 offset;

    private List<System.Action> callOnTrigger = new List<System.Action>();
    private List<System.Action> callOnReset = new List<System.Action>();

    public void Trigger()
    {
        Vector2 v = transform.position;
        v += offset;
        Camera.main.GetComponent<CameraFollow>().OverrideFollow(v);
        foreach (System.Action a in callOnTrigger)
            a();
    }

    public void Reset()
    {
        Camera.main.GetComponent<CameraFollow>().ResetFollowOverride();
        foreach (System.Action a in callOnReset)
            a();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        if (triggerWithCollision)
            Trigger();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        if (resetWithCollision)
            Reset();
    }

    public void CallOnTrigger(System.Action a) { callOnTrigger.Add(a); }
    public void CallOnReset(System.Action a) { callOnReset.Add(a); }
}
