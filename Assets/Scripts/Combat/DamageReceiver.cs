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
    private Material mat, flashMat;
    private Vector2 v, prevVel;
    private RigidbodyConstraints2D prevCon;

    private float stunlockCounter = 0, flashTimer = 0, animPauseTimer = 0, apt2 = 0;
    private bool lightAnim = false, heavyAnim = false, deathAnim = false, dead = false;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if ((recoil & KnockbackTypes.animation) != 0 && anim)
        {
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

        mat = rend.sharedMaterial;
        flashMat = new Material(mat.shader);
        SetFlashMatColors();

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

        SetFlashMatColors();
        rend.material = flashMat;
        if (death && !dead)
        {
            dead |= death;
            PlayerController pCon = GetComponent<PlayerController>();
            flashTimer = 6 / 16f;
            invulnerable = true;
            pauseAnimation(5);
            if (pCon && !DelayedSceneTransition.loading)
            {
                DelayedSceneTransition.loading = true;
                DelayedSceneTransition t = Instantiate(new GameObject()).
                    AddComponent<DelayedSceneTransition>();
                t.delay = 4;
                t.targetScene = "Hub";
                t.loadAsynchronously = true;
                Destroy(pCon);
            }
            Die(point, amount);
            return;
        }

        switch (dtype)
        {
            case DamageType.light:
                flashTimer = 2 / 16f;
                pauseAnimation(2);
                break;
            case DamageType.heavy:
                flashTimer = 5 / 16f;
                pauseAnimation(5);
                break;
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
        health.OnDeath();
        v = point;
        invulnerable = true;

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
        prevVel = rb.velocity;

        gameObject.AddComponent<CorpseBehavior>();

        //Destroy(this);
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
        prevVel = rb.velocity;
        gameObject.AddComponent<CorpseBehavior>();
        //Destroy(this);
    }

    private void Update()
    {
        if(rend.sharedMaterial != mat && !dead)
        {
            if (flashTimer > 0)
                flashTimer -= Time.deltaTime;
            else
                rend.material = mat;
        }

        if(apt2 > 0)
        {
            apt2 -= Time.deltaTime;
            if (apt2 <= 0 && anim)
                anim.speed = 0;
        }

        if(anim && anim.speed < 1)
        {
            if (animPauseTimer > 0)
                animPauseTimer -= Time.deltaTime;
            else
            {
                if(anim) anim.speed = 1;
                rb.constraints = prevCon;
                rb.velocity = prevVel;
            }
        }
    }

    private void SetFlashMatColors()
    {
        flashMat.SetColor("_MonoCol", ReverseColour(mat.GetColor("_MonoCol")));
        flashMat.SetColor("_MainCol", ReverseColour(mat.GetColor("_MainCol")));
    }

    private Color ReverseColour(Color c)
    {
        return new Color(1 - c.r, 1 - c.g, 1 - c.b);
    }

    public void pauseAnimation(int frames)
    {
        prevVel = rb.velocity;
        if (rb.constraints != RigidbodyConstraints2D.FreezeAll)
            prevCon = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        apt2 = 3 / 32f;
        animPauseTimer = frames / 16f;
    }
}