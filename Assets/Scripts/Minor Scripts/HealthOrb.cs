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
        rb.velocity = v;
        rb.angularVelocity = Random.Range(-90, 90) * initialVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health h = collision.gameObject.GetComponent<Health>();
        if (!h)
            return;

        h.Heal(healAmount);

        Destroy(gameObject);
    }
}
