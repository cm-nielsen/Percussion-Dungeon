using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    private Animator anim;

    private bool invulnerable = false;
    private bool lightAnim = false, heavyAnim = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        foreach(AnimatorControllerParameter p in anim.parameters)
        {
            if (p.name == "light hit")
                lightAnim = true;
            if (p.name == "heavy hit")
                heavyAnim = true;
        }
    }

    public void TakeDamage(DamageType dtype, float amount)
    {
        if (invulnerable)
            return;

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

public enum DamageType { light, heavy};
