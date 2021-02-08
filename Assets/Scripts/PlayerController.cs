using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ControlKey input;
    public Transform jumpChecker;
    public Vector2 moveForce;
    public LayerMask isGround;
    public float maxFallSpeed, throwForce;

    private GameObject thrownDrum;
    private Rigidbody2D rb;
    private SpriteRenderer rend;
    private Animator anim;

    public bool canJump, canDodge;

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
        canDodge = true;

        input = GameObject.Find("Player Control Key").GetComponent<ControlKey>();

        Music.onBeat.Add(OnBeat);
        Music.offBeat.Add(OffBeat);
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
            rb.velocity += moveForce.x * Vector2.right;
        else if (input["left"] && !canJump && rend.flipX)
            rb.velocity += moveForce.x * Vector2.left;

        if (rb.velocity.y > maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

        if (input["jump"])
            anim.SetTrigger("jump");
    }

    private void UpdateAnimator()
    {
        canJump = Physics2D.OverlapBox(jumpChecker.position, jumpChecker.localScale, 0, isGround);

        if (input["attack"])
            anim.SetTrigger("attack");
        if (input["alt attack"])
            anim.SetTrigger("alt attack");
        if (input["dodge"] && canJump && canDodge)
        {
            canDodge = false;
            anim.SetTrigger("dodge");
        }
        if ((input["right"] && !input["left"] && rend.flipX) ||
            (input["left"] && !input["right"] && !rend.flipX))
        {
            anim.SetBool("run", false);
            anim.SetBool("turn", true);
        }
        else
        {
            anim.SetBool("turn", false);
            anim.SetBool("run", (input["left"] && !input["right"]) || (input["right"] && !input["left"]));
        }
        anim.SetFloat("vy", rb.velocity.y);
        anim.SetBool("ground", canJump);
        anim.SetBool("down", input["down"]);
    }

    [HideInInspector]
    private void OnBeat()
    {
        anim.SetBool("beat", true);
    }
    [HideInInspector]
    public void OffBeat()
    {
        anim.SetBool("beat", false);
    }

    // --------------------------------------------------------------------------------

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

    private void FinishDodge()
    {
        canDodge = true;
    }

    private void TurnTowardsInput()
    {
        if (input["right"])
        {
            rend.flipX = false;
        }
        else if (input["left"])
        {
            rend.flipX = true;
        }
    }
}