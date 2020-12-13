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
    private Animator anim;

    private bool canJump;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    // is caled at a fixed rate
    void FixedUpdate()
    {
        rb.velocity *= Vector2.up;
        if (input["right"] && !rend.flipX)
        {
            //anim.SetBool()
            //rb.velocity += moveForce.x * Vector2.right;
            //rend.flipX &= false;
        }
        else if (input["left"])
        {
            //rb.velocity += moveForce.x * Vector2.left;
            //rend.flipX = true;
        }

        if (rb.velocity.y > maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

        if (input["up"])
            rb.AddForce(moveForce.y * Vector2.up, ForceMode2D.Impulse);
    }

    private void UpdateAnimator()
    {
        if (input["attack"])
            anim.SetTrigger("attack");
        if (input["special attack"])
            anim.SetTrigger("special attack");
        if ((input["right"] && rend.flipX) || (input["left"] && !rend.flipX))
        {
            anim.SetBool("run", false);
            //anim.SetBool("run", input["left"] || input["right"]);
            anim.SetBool("turn", true);
        }
        else
        {
            anim.SetBool("turn", false);
            anim.SetBool("run", input["left"] || input["right"]);
        }
    }

    // --------------------------------------------------------------------------------
    private void Move(int x)
    {
        if(rend.flipX)
            transform.position += x * Vector3.left * 0.03125f;
        else
            transform.position += x * Vector3.right * 0.03125f;
    }

    private void Flip()
    {
        rend.flipX = !rend.flipX;
    }

    private void SpawnObject(GameObject g)
    {
        Instantiate(g, transform.position, Quaternion.identity);
    }
}