using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public LayerMask isGround;
    public bool rebound, catchable;

    private Rigidbody2D rb;
    private Vector2 reboundVelocity;

    private void Start()
    {
        if (rebound)
        {
            rb = GetComponent<Rigidbody2D>();
            reboundVelocity = -rb.velocity;
        }
        catchable = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isGround == (isGround | ( 1 << collision.gameObject.layer)))
        {
            GetComponent<Animator>().SetBool("hit", true);
            catchable = true;
        }
    }

    private void Rebound()
    {
        GetComponent<Animator>().SetBool("hit", false);
        rb.velocity = reboundVelocity;
        reboundVelocity *= -1;
        GetComponentInChildren<DamageDealer>().StartSwing();
    }
}
