using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimationMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Flip()
    {
        //rend.flipX = !rend.flipX;
        transform.localScale = new Vector2(-transform.localScale.x, 1);
    }

    private void Move(int x)
    {
        if (rend.flipX || transform.localScale.x < 0)
            rb.MovePosition(transform.position + x * Vector3.left * 0.03125f);
        else
            rb.MovePosition(transform.position + x * Vector3.right * 0.03125f);
    }

    private void VerticalImpulse(float y)
    {
        rb.velocity *= Vector2.right;
        rb.velocity += Vector2.up * y;
    }

    private void HorizontalImpulse(float x)
    {
        if (rend.flipX || transform.localScale.x < 0)
            rb.velocity += Vector2.left * x;
        else
            rb.velocity += Vector2.right * x;
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
}
