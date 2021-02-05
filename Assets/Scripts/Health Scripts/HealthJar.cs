using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthJar : DamageReceiver
{
    public GameObject healthOrbPrefab;
    public AudioClip breakSound;
    public int orbs;

    private ParticleSystem pSystem;
    private AudioSource aSource;
    private bool killable = false;

    private void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        aSource = gameObject.AddComponent<AudioSource>();
        AudioClipPlayer.ApplyParameters(aSource);

        aSource.clip = breakSound;
    }

    private void Update()
    {
        if (killable && !pSystem.IsAlive() && !aSource.isPlaying)
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
        aSource.Play();
        killable = true;
    }
}
