using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ControlKey input;
    public Vector2 moveForce;
    public float maxFallSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer rend;

    private bool canJump;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity *= Vector2.up;
        if (input["right"])
        {
            rb.velocity += moveForce.x * Vector2.right;
            rend.flipX = false;
        }
        else if (input["left"])
        {
            rb.velocity += moveForce.x * Vector2.left;
            rend.flipX = true;
        }

        if (rb.velocity.y > maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

        if (input["up"])
            rb.AddForce(moveForce.y * Vector2.up, ForceMode2D.Impulse);
    }
}
