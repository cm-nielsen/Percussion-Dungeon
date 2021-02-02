using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthJar : DamageReceiver
{
    public GameObject healthOrbPrefab;
    public int orbs;

    private ParticleSystem pSystem;
    private bool killable = false;

    private void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (killable && !pSystem.IsAlive())
            Destroy(gameObject);
    }

    public override void TakeDamage(DamageType dtype, float amount, Vector2 point)
    {
        while(orbs > 0)
        {
            Instantiate(healthOrbPrefab, transform.position, Quaternion.identity);
            orbs--;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        pSystem.Play();
        killable = true;
    }
}
