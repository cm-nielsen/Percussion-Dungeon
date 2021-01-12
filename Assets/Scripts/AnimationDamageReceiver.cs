using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDamageReceiver : DamageReceiver
{
    private Animator anim;
    private SpriteRenderer rend;

    private bool lightAnim = false, heavyAnim = false;
    // Start is called before the first frame update
    void Start()
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

    public override void TakeDamage(DamageType dtype, float amount, Vector2 point)
    {
        if (invulnerable)
            return;

        //health - amount;
        //Debug.Log(gameObject.name + " took " + amount + " damage");

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

        if (damageTextPrefab)
        {
            GameObject g = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            LerpFromPoint l = g.GetComponent<LerpFromPoint>();
            l.Initiate(amount);
        }
    }
}
