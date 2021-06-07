using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangController : MonoBehaviour
{
    public float throwForce = 10, rollForce, rollTorque, pickupOffset;

    private PlayerController pCon;
    private GameObject thrownObject;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer rend;
    private BoxCollider2D hitbox;
    private CircleCollider2D rollHitbox;

    private float circumference, prevX;
    private bool rolling = false;
    // Start is called before the first frame update
    void Start()
    {
        pCon = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<BoxCollider2D>();
        rollHitbox = GetComponent<CircleCollider2D>();
        rollHitbox.enabled = false;
        circumference = rollHitbox.radius * 2 * Mathf.PI;
    }

    // Update is called once per frame
    void Update()
    {
        if (rolling)
        {
            float deg = 360 * (transform.position.x - prevX) / circumference;
            rb.rotation -= deg;
        }
        prevX = transform.position.x;
        anim.ResetTrigger("recover");

        if (thrownObject)
        {
            Collider2D[] cols = new Collider2D[100];
            hitbox.GetContacts(cols);
            foreach (Collider2D c in cols)
            {
                if (!c)
                    break;
                if (c.transform.root == thrownObject.transform.root)
                    anim.SetTrigger("recover");
            }
        }
    }


    private void ThrowDrum(GameObject prefab)
    {
        thrownObject = Instantiate(prefab, transform.position, Quaternion.identity);
        thrownObject.GetComponent<SpriteRenderer>().flipX = rend.flipX;
        Rigidbody2D r = thrownObject.GetComponent<Rigidbody2D>();
        thrownObject.GetComponentInChildren<DamageDealer>().
            SetSelfReceiver(GetComponent<DamageReceiver>());

        anim.SetBool("naked", true);

        if (r)
            r.velocity = rb.velocity + throwForce * Vector2.down;
    }

    private void RetrieveDrum()
    {
        Vector2 pos = thrownObject.transform.position;
        if (thrownObject.GetComponent<SpriteRenderer>().flipX)
            pos.x -= 14 / 32f;
        else
            pos.x += 14 / 32f;

        transform.position = pos;
        anim.SetBool("naked", false);
        Destroy(thrownObject);
    }

    private void StartRoll()
    {
        rollHitbox.enabled = true;
        rolling = true;
        hitbox.enabled = false;
        pCon.enabled = false;

        if (rend.flipX)
            rb.AddForce(Vector2.left * rollForce, ForceMode2D.Impulse);
        else
            rb.AddForce(Vector2.right * rollForce, ForceMode2D.Impulse);
    }

    private void EndRoll()
    {
        hitbox.enabled = true;
        pCon.enabled = true;
        rollHitbox.enabled = false;
        rolling = false;
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.transform.root.gameObject == thrownObject)
    //        anim.SetTrigger("recover");
    //}
}
