using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageDealer : MonoBehaviour
{
    public DamageType dType = DamageType.light;
    public AudioClip[] heavyNoise, lightNoise;
    public float movementValue = 1, vampMultiplier = 0;

    public float verticalRecoil = .5f;


    private List<Collider2D> ignore = new List<Collider2D>(),
        exceptions = new List<Collider2D>();
    private Health health;
    private DamageReceiver selfReciever;
    private SpriteRenderer parentSprite;
    private float damageMultiplier = 1, heldValue = 0;
    private bool useHeldValue = false;

    private void Start()
    {
        if (!selfReciever)
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
        //// you might be tempted to uncomment this,
        //// In almost every way, it would seem like a great fix
        //// fuck you it breaks all damage
        //if (!enabled)
        //    return;

        if (ignore.Contains(collision))
            return;

        if (exceptions.Contains(collision))
            return;

        if (selfReciever &&
            collision.transform.root == selfReciever.transform.root)
            return;

        if (selfReciever)
            selfReciever.ResetStunCount();

        ignore.Add(collision);
        DamageReceiver[] rec = collision.GetComponents<DamageReceiver>();

        Vector2 dir = GetComponent<BoxCollider2D>().offset;
        //dir -= (Vector2)transform.parent.position;
        if (parentSprite && parentSprite.flipX)
            dir.x *= -1;
        dir.x += transform.lossyScale.x;

        dir += Vector2.up * verticalRecoil;

        if (rec == null || rec.Length == 0)
            return;


        bool miffed = false;
        foreach (DamageReceiver r in rec)
            miffed |= !r.TakeDamage(dType,
                (useHeldValue ? heldValue : movementValue)
                * damageMultiplier, dir);

        if (miffed) // don't pause/screenshake unless a hit is landed
            return;

        Camera.main.GetComponent<CameraFollow>().Shake(dir,
            useHeldValue ? heldValue : movementValue);
        if (selfReciever)
        {
            switch (dType)
            {
                case DamageType.light:
                    selfReciever.pauseAnimation(0);
                    AudioClipPlayer.PlayRandom(lightNoise);
                    break;
                case DamageType.heavy:
                    selfReciever.pauseAnimation(4);
                    AudioClipPlayer.PlayRandom(heavyNoise);
                    break;
            }
        }

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
        ignore.Clear();
        if (selfReciever)
            selfReciever.ResetStunCount();
    }

    public void SetSelfReceiver(DamageReceiver r)
    {
        selfReciever = r;
    }

    public void SetDamageMultiplier()
    {

        damageMultiplier = GameController.GetDamageMod();
    }

    public void AddException(Collider2D c)
    {
        exceptions.Add(c);
    }

    public void HoldValue(float f)
    {
        heldValue = f;
        useHeldValue = true;
    }

    public void ReleaseHeldValue()
    {
        heldValue = movementValue;
        useHeldValue = false;
    }
}
