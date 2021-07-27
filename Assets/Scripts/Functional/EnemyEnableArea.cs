using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEnableArea : MonoBehaviour
{
    public LayerMask enemies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior e = collision.GetComponent<EnemyBehavior>();
            if (e)
                e.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior e = collision.GetComponent<EnemyBehavior>();
            if (e)
                e.enabled = false;
        }
    }
}
