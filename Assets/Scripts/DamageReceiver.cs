using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { light, heavy};

public class DamageReceiver : MonoBehaviour
{
    [System.Flags]
    public enum KnockbackTypes { animation, physics}
    public KnockbackTypes recoil;

    public GameObject damageTextPrefab;
    public bool invulnerable, animationRecoil, physicsRecoil;

    private Health health;
    private Animator anim;
    private SpriteRenderer rend;
    private Rigidbody2D rb;

    public float knockbackStrengthMod = 1;
    private bool lightAnim = false, heavyAnim = false;

    private void Start()
    {
        if (animationRecoil)
        {
            anim = GetComponent<Animator>();
            rend = GetComponent<SpriteRenderer>();

            foreach (AnimatorControllerParameter p in anim.parameters)
            {
                if (p.name == "light hit")
                    lightAnim = true;
                if (p.name == "heavy hit")
                    heavyAnim = true;
            }
        }
        if (physicsRecoil)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (!health)
            health = GetComponent<Health>();
    }

    public void TakeDamage(DamageType dtype, float amount, Vector2 point)
    {
        if (invulnerable)
            return;

        if(health)
            health.Reduce(amount);

        if(animationRecoil)
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

        if(physicsRecoil)
        {
            rb.AddForce((point.normalized) * amount * knockbackStrengthMod,
            ForceMode2D.Impulse);
        }

        if (damageTextPrefab)
        {
            GameObject g = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            LerpFromPoint l = g.GetComponent<LerpFromPoint>();
            l.Initiate(amount);
        }
    }
}