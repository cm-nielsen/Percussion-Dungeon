using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ControlKey input;
    public Transform jumpChecker;
    public Vector2 moveForce;
    public LayerMask isGround;
    public float maxFallSpeed, throwForce;

    private GameObject thrownDrum;
    private Rigidbody2D rb;
    private SpriteRenderer rend;
    private Animator anim;

    public bool canJump;

    /// <summary>
    /// Draws shapes in the inpsector window, used to show jump/wall "hitboxes"
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawCube(jumpChecker.position, jumpChecker.localScale);
    }
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
        if (input["right"] && !canJump && !rend.flipX)
        {
            //anim.SetBool()
            //rb.velocity += moveForce.x * Vector2.right;
            //rend.flipX = false;
            if(!canJump)
                rb.velocity += moveForce.x * Vector2.right;
        }
        else if (input["left"] && !canJump && rend.flipX)
        {
            //rb.velocity += moveForce.x * Vector2.left;
            //rend.flipX = true;
            if (!canJump)
                rb.velocity += moveForce.x * Vector2.left;
        }

        if (rb.velocity.y > maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

        if (input["up"])
            anim.SetTrigger("jump");
            //rb.AddForce(moveForce.y * Vector2.up, ForceMode2D.Impulse);
    }

    private void UpdateAnimator()
    {
        canJump = Physics2D.OverlapBox(jumpChecker.position, jumpChecker.localScale, 0, isGround);
        //Debug.Log(Physics2D.OverlapBox(jumpChecker.position, jumpChecker.localScale, 0, isGround));

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
        anim.SetFloat("vy", rb.velocity.y);
        anim.SetBool("ground", canJump);
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

    private void Jump()
    {
        Vector2 dir = moveForce.y * Vector2.up;
        if (input["right"])
            dir += moveForce.x * Vector2.right;
        if (input["left"])
            dir += moveForce.x * Vector2.left;
        rb.AddForce(dir, ForceMode2D.Impulse);
    }

    private void LockPosition(int i)
    {
        if (i == 0)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        else if (i == 1)
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else if (i == 2)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void VerticalImpulse(float y)
    {
        rb.velocity *= Vector2.right;
        rb.velocity += Vector2.up * y;
    }

    private void ThrowDrum(GameObject prefab)
    {
        thrownDrum = Instantiate(prefab, transform.position, Quaternion.identity);
        thrownDrum.GetComponent<SpriteRenderer>().flipX = rend.flipX;
        Rigidbody2D r = thrownDrum.GetComponent<Rigidbody2D>();

        if (r)
            r.velocity = rb.velocity + throwForce * Vector2.down;
    }

    private void RetrieveDrum()
    {
        Destroy(thrownDrum);
    }
}