using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public float healAmount = 1, initialVelocity, magnetism;
    public AudioClip noise;

    private Rigidbody2D rb;
    private Transform pPos;

    private void Start()
    {
        Vector2 v = Random.insideUnitCircle * initialVelocity;
        if (v.y < 0)
            v.y *= -1;

        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            rb = GetComponentInParent<Rigidbody2D>();
        if (!rb)
            return;

        rb.velocity = v;
        rb.angularVelocity = Random.Range(-90, 90) * initialVelocity;
    }

    private void Update()
    {
        if (!pPos)
        {
            PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
            if (pCon) pPos = pCon.transform;
            return;
        }

        Vector2 dir = (pPos.position - transform.position);
        rb.AddForce(dir.normalized * magnetism * Time.deltaTime / dir.magnitude);
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        Health h = c.GetComponent<Health>();
        if (!h)
            return;

        if (h.Heal(healAmount))
        {
            GetComponentInParent<Animator>().SetTrigger("start");
            if (noise)
                AudioClipPlayer.Play(noise);
            this.enabled = false;
        }
    }
}
