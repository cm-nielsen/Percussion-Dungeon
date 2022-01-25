using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationMovement))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(DamageReceiver))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(AudioClipPlayer))]
public class PlayerController : MonoBehaviour
{
    private ControlKey input;

    public Vector2 moveForce = new Vector2(22, 9.5f);
    public LayerMask isGround;
    public float maxFallSpeed = 100, xFriction = 0.8f,
        coyoteTime = 0.2f, adaptiveGravity = 5, runSpeed = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer rend;
    private Animator anim;
    private BoxCollider2D hitbox;
    private Vector2 normalmoveForce;

    private float cTimer = Mathf.Infinity;
    private bool canJump, canDodge, grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        canDodge = true;
        grounded = false;

        input = GameObject.Find("Player Control Key").GetComponent<ControlKey>();

        UpdateDamage();

        Music.onBeat.Add(OnBeat);
        Music.offBeat.Add(OffBeat);
        normalmoveForce = moveForce;
    }

    private void Update()
    {
        UpdateAnimator();
    }

    // is caled at a fixed rate
    void FixedUpdate()
    {
        rb.AddForce(Mathf.Abs(rb.velocity.y) * adaptiveGravity * Vector2.down);
        AnimatorClipInfo[] animInfo = anim.GetCurrentAnimatorClipInfo(0);
        string currentAnimation = "none";
        if(animInfo.Length > 0)
            currentAnimation = animInfo[0].clip.name;
        currentAnimation = currentAnimation.ToLower();
        if (grounded)
        {
            bool landing = (currentAnimation.Contains("land") &&
                !currentAnimation.Contains("attack"));
            if (currentAnimation.Contains("run") || landing)
                ApplyRunForce(!landing);

            rb.velocity = new Vector2(rb.velocity.x * xFriction, rb.velocity.y);
            if (rb.velocity.x > runSpeed)
                rb.velocity = new Vector2(runSpeed, rb.velocity.y);
            else if(rb.velocity.x < -runSpeed)
                rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
        }
        else
        {
                AirControl();
            if (rb.velocity.y < -maxFallSpeed)
                rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);

            if (currentAnimation.Contains("jump") || currentAnimation.Contains("fall"))
                TurnTowardsInput();
        }
    }
    [HideInInspector]
    private void UpdateAnimator()
    {
        grounded = false;
        Vector3 groundCheckOffset = hitbox.size / 2 * Vector2.right, hOff = hitbox.offset;
        hOff.x *= transform.localScale.x;
        if(Physics2D.Raycast(transform.position + groundCheckOffset + hOff, Vector2.down, 
            hitbox.size.y / 2 + 0.05f, isGround)
        || Physics2D.Raycast(transform.position - groundCheckOffset + hOff, Vector2.down, 
        hitbox.size.y / 2 + 0.05f, isGround))
        {
            canJump = true;
            grounded = true;
            cTimer = 0;
        }
        else
        {
            canJump = cTimer < coyoteTime;
            cTimer += Time.deltaTime;
        }

        anim.SetBool("jump", input["jump"] && canJump);

        if (input["attack"])
            anim.SetTrigger("attack");
        if (input["alt attack"])
            anim.SetTrigger("alt attack");

        if (input["dodge"] && canJump && canDodge)
        {
            canDodge = false;
            anim.SetTrigger("dodge");
        }

        anim.SetFloat("vy", rb.velocity.y);
        anim.SetFloat("vx", Mathf.Abs(rb.velocity.x));
        anim.SetBool("ground", grounded);
        anim.SetBool("down", input["down"]);

        anim.SetBool("run", input["left"] || input["right"]);
    }
    [HideInInspector]
    private void ApplyRunForce(bool b = true)
    {
        if (input["right"])
        {
            if (transform.localScale.x < 0)
            {
                if (b)
                {
                    anim.SetTrigger("turn");
                    transform.localScale = new Vector2(1, 1);
                }
            }
            rb.AddForce(moveForce.x * Vector2.right);

        }
        else if (input["left"])
        {
            if (transform.localScale.x > 0)
            {
                if (b)
                {
                    anim.SetTrigger("turn");
                    transform.localScale = new Vector2(-1, 1);
                }
            }
            rb.AddForce(moveForce.x * Vector2.left);
        }
        //TurnTowardsInput();
    }
    [HideInInspector]
    private void AirControl()
    {
        if (input["right"])
            rb.AddForce(moveForce.x * Vector2.right);
        else if (input["left"])
            rb.AddForce(moveForce.x * Vector2.left);

        if (cTimer > coyoteTime/2)
            rb.velocity = new Vector2(rb.velocity.x * Mathf.Pow(xFriction, .6f), rb.velocity.y);
        else if(rb.velocity.y <= 0)
            rb.velocity *= Vector2.right * xFriction;
    }
    [HideInInspector]
    public void UpdateDamage()
    {
        GetComponentInChildren<DamageDealer>().SetDamageMultiplier();
    }

    [HideInInspector]
    private void OnBeat()
    {
        if(anim)
            anim.SetBool("beat", true);
    }
    [HideInInspector]
    public void OffBeat()
    {
        if(anim)
            anim.SetBool("beat", false);
    }

    // --------------------------------------------------------------------------------

    private void SpawnObject(GameObject g)
    {
        GameObject inst = Instantiate(g, transform.position, Quaternion.identity);
        inst.transform.localScale = transform.localScale;
    }

    private void Jump()
    {
        rb.velocity *= Vector2.right;
        Vector2 dir = moveForce.y * Vector2.up;

        rb.AddForce(dir, ForceMode2D.Impulse);
        GetComponent<DamageReceiver>().ReceiveImpulse(dir);
        canJump = false;
        anim.SetBool("ground", false);
        cTimer = coyoteTime;
    }

    private void FinishDodge()
    {
        canDodge = true;
    }

    private void TurnTowardsInput()
    {
        if (input["right"])
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if (input["left"])
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void CheckContact()
    {
        anim.SetBool("contact", GetComponentInChildren<AreaCollisionCheck>().Check());
    }
    /// <summary>
    /// Consume attack input triggers for frame perfect input combos,
    /// 0 for basic, -1 for alt only, 1 for both
    /// </summary>
    private void ConsumeAttackInputs(int n = 0)
    {
        bool b;
        if (n >= 0)
        {
            b = input["attack"];
            anim.ResetTrigger("attack");
        }
        if (n > 0 || n < 0)
        {
            b = input["alt attack"];
            anim.ResetTrigger("alt attack");
        }
    }

    private void SetMoveForce(string s)
    {
        string[] ar = s.Split(',');
        if(ar.Length != 2)
        {
            moveForce = normalmoveForce;
            return;
        }
        moveForce = new Vector2(float.Parse(ar[0]), float.Parse(ar[1]));
    }

    private void SetChargeAttackValue(float f)
    {
        GetComponentInChildren<DamageDealer>().HoldValue(f);
    }

    //private void StoreChargeAttackValue(float f)
    //{

    //}

    private void AddToChargeAttackValue(float f)
    {
        GetComponentInChildren<DamageDealer>().AddToHeldValue(f, false);
    }

    private void UseChargeAttackValue()
    {
        GetComponentInChildren<DamageDealer>().UseHeldValue();
    }

    private void ChargeAttackEnd()
    {
        GetComponentInChildren<DamageDealer>().ReleaseHeldValue();
    }
}