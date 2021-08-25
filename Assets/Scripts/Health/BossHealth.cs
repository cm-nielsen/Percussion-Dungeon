using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health
{
    public GameObject expOrbPrefab;
    public int experience;//, group;

    public override bool Reduce(float dam)
    {
        if (amount - dam < max / 2)
            GetComponent<BossBehavior>().OnHalfHealth();
        return base.Reduce(dam);
    }

    public override void OnDeath()
    {
        if (!expOrbPrefab) return;
        for (int i = 0; i < experience; i++)
            Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
        expOrbPrefab = null;

        //if (display)
        //    Destroy(display.gameObject);

        BossBehavior bhv = GetComponent<BossBehavior>();
        bhv.OnDeath();
        bhv.MidRollBite();

        GetComponent<Animator>().SetTrigger("die");
        Destroy(GetComponentInChildren<DamageDealer>().gameObject);
        Destroy(bhv);

        BossAttackTrigger[] attackTriggers = GetComponentsInChildren<BossAttackTrigger>();
        for (int i = 0; i < attackTriggers.Length; i++)
            Destroy(attackTriggers[i].gameObject);
    }
}
