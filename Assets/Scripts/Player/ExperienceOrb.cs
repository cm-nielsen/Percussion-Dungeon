using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public float initialVelocity, magnetism, collectionDelay;
    public int amount = 1;
    public AudioClip noise;

    private Rigidbody2D rb;
    private Transform pPos;

    private float timer = 0;

    private void Start()
    {
        Vector2 v = Random.insideUnitCircle * initialVelocity;
        if (v.y < 0)
            v.y *= -1;

        rb = GetComponent<Rigidbody2D>();

        rb.velocity = v;
        rb.angularVelocity = Random.Range(-90, 90) * initialVelocity;
    }

    private void Update()
    {
        if(timer < collectionDelay)
        {
            timer += Time.deltaTime;
            return;
        }

        if (!pPos)
        {
            PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
            if (pCon) pPos = pCon.transform;
            return;
        }

        rb.AddForce((pPos.position - transform.position) * magnetism);
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (timer < collectionDelay)
            return;
        if (!c.GetComponent<PlayerController>())
            return;

        GameController.GainExp(amount);
        GetComponentInParent<Animator>().SetTrigger("start");
        if (noise)
            AudioClipPlayer.Play(noise);
        this.enabled = false;
    }
}
