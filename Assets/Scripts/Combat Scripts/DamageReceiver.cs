using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { light, heavy};

public class DamageReceiver : MonoBehaviour
{
    [System.Flags]
    public enum KnockbackTypes { none = 0, animation = 1, physics = 2, breakable = 4 }
    public KnockbackTypes recoil;

    public GameObject damageTextPrefab;
    public float knockbackStrengthMod = 1, bullyability = 4, deathForce = 10;
    public bool invulnerable;

    private Health health;
    private Animator anim;
    private SpriteRenderer rend;
    private Rigidbody2D rb;
    private Vector2 v;

    private float stunlockCounter = 0;
    private bool lightAnim = false, heavyAnim = false, deathAnim = false;

    private void Start()
    {
        if ((recoil & KnockbackTypes.animation) != 0)
        {
            anim = GetComponent<Animator>();
            rend = GetComponent<SpriteRenderer>();

            foreach (AnimatorControllerParameter p in anim.parameters)
            {
                if (p.name == "light hit")
                    lightAnim = true;
                if (p.name == "heavy hit")
                    heavyAnim = true;
                if (p.name == "die")
                    deathAnim = true;
            }
        }

        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        //animationRecoil = (recoil & KnockbackTypes.animation) != 0;
        //physicsRecoil = (recoil & KnockbackTypes.physics) != 0;
    }

    public virtual void TakeDamage(DamageType dtype, float amount, Vector2 point)
    {
        if (invulnerable)
            return;

        bool death = false;
        if(health)
            death = health.Reduce(amount);


        if (damageTextPrefab)
        {
            GameObject g = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            LerpFromPoint l = g.GetComponent<LerpFromPoint>();
            l.Initiate(amount);
        }

        if (death)
        {
            Die(point, amount);
            return;
        }

        Recoil(dtype, amount, point, stunlockCounter >= bullyability && bullyability > 0);
        stunlockCounter += amount;
    }

    public void ResetStunCount()
    {
        stunlockCounter = 0;
    }

    private void Recoil(DamageType dtype, float amount, Vector2 point, bool bully)
    {
        if((recoil & KnockbackTypes.physics) != 0)
        {
            Vector2 v = (point.normalized) * amount * knockbackStrengthMod;
            if (bully)
                v /= 2;
            rb.AddForce(v, ForceMode2D.Impulse);
        }

        if (bully)
            return;

        if((recoil & KnockbackTypes.animation) != 0)
        {
            rend.flipX = point.x > 0;
            switch (dtype)
            {
                case DamageType.light:
                    if (lightAnim)
                        anim.SetTrigger("light hit");
                    break;
                case DamageType.heavy:
                    if (heavyAnim)
                        anim.SetTrigger("heavy hit");
                    break;
            }
        }

    }

    private void Die(Vector2 point, float amount)
    {
        v = point;

        if ((recoil & KnockbackTypes.animation) != 0)
        {
            rend.flipX = point.x > 0;
            if (deathAnim)
                anim.SetTrigger("die");
        }

        if ((recoil & KnockbackTypes.physics) != 0)
            AddDeathForce(amount);
    }

    public void AddDeathForce()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        if (!rb)
            return;

        v += Vector2.up;
        //rb.sharedMaterial = deadMeat;
        rb.AddForce((v.normalized) * deathForce, ForceMode2D.Impulse);

        gameObject.AddComponent<CorpseBehavior>();

        Destroy(this);
    }

    public void AddDeathForce(float amount)
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        if (!rb)
            return;

        v *= 4;
        v = (v.normalized) * (deathForce + amount * knockbackStrengthMod);
        rb.AddForce(v, ForceMode2D.Impulse);
        gameObject.AddComponent<CorpseBehavior>();
        Destroy(this);
    }
}