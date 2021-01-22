using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { light, heavy};

public class DamageReceiver : MonoBehaviour
{
    [System.Flags]
    public enum KnockbackTypes { none, animation, physics }
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

    public void TakeDamage(DamageType dtype, float amount, Vector2 point)
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
            Die(point);
            return;
        }

        stunlockCounter += amount;
        if (stunlockCounter >= bullyability && bullyability > 0)
            return;

        Recoil(dtype, amount, point);
    }

    public void ResetStunCount()
    {
        stunlockCounter = 0;
    }

    private void Recoil(DamageType dtype, float amount, Vector2 point)
    {
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

        if((recoil & KnockbackTypes.physics) != 0)
        {
            rb.AddForce((point.normalized) * amount * knockbackStrengthMod,
            ForceMode2D.Impulse);
        }
    }

    private void Die(Vector2 point)
    {
        v = point;

        if ((recoil & KnockbackTypes.animation) != 0)
        {
            rend.flipX = point.x > 0;
            if (deathAnim)
                anim.SetTrigger("die");
        }

        if ((recoil & KnockbackTypes.physics) != 0)
            AddDeathForce();
    }

    public void AddDeathForce()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        if (!rb)
            return;

        v *= 4;
        //rb.sharedMaterial = deadMeat;
        rb.AddForce((v.normalized) * deathForce, ForceMode2D.Impulse);

        gameObject.AddComponent<CorpseBehavior>();

        Destroy(this);
    }
}