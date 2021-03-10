using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public DamageType dType = DamageType.light;
    public float movementValue = 1, vampMultiplier = 0;


    private List<Collider2D> ignore = new List<Collider2D>();
    private Health health;
    private DamageReceiver selfReciever;
    private SpriteRenderer parentSprite;
    private float damageMultiplier = 1;

    private void Start()
    {
        selfReciever = GetComponentInParent<DamageReceiver>();
        parentSprite = GetComponentInParent<SpriteRenderer>();
        health = GetComponentInParent<Health>();

        if (parentSprite)
            if (parentSprite.flipX)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ignore.Contains(collision))
            return;

        if (selfReciever)
            selfReciever.ResetStunCount();

        ignore.Add(collision);
        DamageReceiver[] rec = collision.GetComponents<DamageReceiver>();

        Vector2 dir = GetComponent<BoxCollider2D>().offset;
        //dir -= (Vector2)transform.parent.position;
        if (parentSprite && parentSprite.flipX)
            dir.x *= -1;
        dir.x += transform.localScale.x;

        dir -= Vector2.up;

        if (rec == null)
            return;

        selfReciever.pauseAnimation(2);

        foreach (DamageReceiver r in rec)
            r.TakeDamage(dType, movementValue * damageMultiplier, dir);

        if(health && vampMultiplier > 0)
            health.Heal(movementValue * damageMultiplier * vampMultiplier);
    }

    public void StartSwing()
    {
        ignore.Clear();
    }

    private void OnEnable()
    {
        ignore.Clear();

        if (parentSprite)
            if (parentSprite.flipX)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);
    }

    private void OnDisable()
    {
        if (selfReciever)
            selfReciever.ResetStunCount();
    }
}
