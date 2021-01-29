using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public float healAmount = 1, initialVelocity;

    private void Start()
    {
        Vector2 v = Random.insideUnitCircle * initialVelocity;
        if (v.y < 0)
            v.y *= -1;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (!rb)
            rb = GetComponentInParent<Rigidbody2D>();
        if (!rb)
            return;

        rb.velocity = v;
        rb.angularVelocity = Random.Range(-90, 90) * initialVelocity;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        Health h = c.GetComponent<Health>();
        if (!h)
            return;

        if (h.Heal(healAmount))
        {
            GetComponentInParent<Animator>().SetTrigger("start");
            this.enabled = false;
        }
    }
}
