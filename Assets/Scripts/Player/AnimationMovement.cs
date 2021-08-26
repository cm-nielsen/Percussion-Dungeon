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
        Vector3 offset = Vector2.zero;
        if (rend.flipX || transform.localScale.x < 0)
            offset = x * Vector3.left * 0.03125f;
        else
            offset = x * Vector3.right * 0.03125f;

        if (rb.constraints != RigidbodyConstraints2D.FreezeAll)
            rb.MovePosition(transform.position + offset);
        else
        {
            transform.Translate(offset);
            rb.MovePosition(transform.position);
        }
    }

    private void VerticalImpulse(float y)
    {
        rb.velocity *= Vector2.right;
        rb.velocity += Vector2.up * y;
        GetComponent<DamageReceiver>().ReceiveImpulse(Vector2.up * y);
    }

    private void HorizontalImpulse(float x)
    {
        Vector2 diff = Vector2.zero;
        if (rend.flipX || transform.localScale.x < 0)
            diff = Vector2.left * x;
        else
            diff = Vector2.right * x;
        rb.velocity += diff;
        GetComponent<DamageReceiver>().ReceiveImpulse(diff);
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

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;
    }

    private void SetBool(string s)
    {
        string[] ar = s.Split(',');
        GetComponent<Animator>().SetBool(ar[0], int.Parse(ar[1]) > 0);
    }
}
