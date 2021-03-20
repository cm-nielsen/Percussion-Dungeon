using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDamageDealer : MonoBehaviour
{
    public Rigidbody2D rb;
    public DamageType dType = DamageType.light;
    public float movementValue = 1;

    private List<Collider2D> ignore = new List<Collider2D>();

    private void OnEnable()
    {
        ignore.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ignore.Contains(collision))
            return;
        //if (rb.velocity.y < .01f)
        //    return;

        float mul = rb.velocity.magnitude + Mathf.Abs(rb.velocity.x);
        //mul *= 10;
        
        ignore.Add(collision);
        DamageReceiver[] rec = collision.GetComponents<DamageReceiver>();

        Vector2 dir = new Vector2(rb.velocity.x, -rb.velocity.y);

        if (rec == null)
            return;

        foreach (DamageReceiver r in rec)
            r.TakeDamage(dType, movementValue * mul, dir);
    }

}
