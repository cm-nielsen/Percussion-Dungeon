using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackReceiver : DamageReceiver
{
    public float knockbackStrengthMod = 1;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void TakeDamage(DamageType dtype, float amount, Vector2 point)
    {
        //health - amount;
        rb.AddForce(((Vector2)transform.position - point).normalized *
            amount * knockbackStrengthMod, ForceMode2D.Impulse);
    }
}
