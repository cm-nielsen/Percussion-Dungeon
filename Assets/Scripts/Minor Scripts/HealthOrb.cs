using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public float healAmount = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health h = collision.gameObject.GetComponent<Health>();
        if (!h)
            return;

        h.Heal(healAmount);

        Destroy(gameObject);
    }
}
