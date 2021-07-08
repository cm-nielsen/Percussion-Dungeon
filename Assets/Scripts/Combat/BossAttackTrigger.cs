using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossAttackTrigger : MonoBehaviour
{
    public string parameterName;
    public bool isBool;
    public float cooldown = 0;

    private Animator anim;
    private float timeTriggered = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        if (!anim)
            anim = transform.parent.GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float t = Time.time;
        if (!collision.CompareTag("Player") || t - timeTriggered < cooldown)
            return;

        timeTriggered = t;
        Toggle(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        Toggle(false);
    }

    private void Toggle(bool b)
    {
        if (!anim)
            return;
        if (isBool)
            anim.SetBool(parameterName, b);
        else if (b)
            anim.SetTrigger(parameterName);
        else
            anim.ResetTrigger(parameterName);
    }
}
