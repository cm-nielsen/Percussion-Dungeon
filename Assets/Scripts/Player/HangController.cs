using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangController : MonoBehaviour
{
    public float throwForce = 10, rollForce, minRollTime;

    private PlayerController pCon;
    private GameObject thrownObject;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer rend;
    private BoxCollider2D hitbox;
    private CircleCollider2D rollHitbox;

    private ControlSteadyProxy roll;

    private float circumference, prevX, rollStart = -100;
    private bool rolling = false, canRoll = true;
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

        roll = new ControlSteadyProxy();
        roll.Setup(GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>(), "dodge");
    }

    // Update is called once per frame
    void Update()
    {
        bool b = roll.val || rollStart + minRollTime > Time.time;
        anim.SetBool("roll", b);
        anim.SetBool("can roll", canRoll);
        if (roll.val && canRoll)
            canRoll = false;

        if (rolling)
        {
            float deg = 360 * (transform.position.x - prevX) / circumference;
            rb.rotation -= deg;
        }
        else
            transform.eulerAngles = new Vector3(0, 0, 0);

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
        thrownObject.transform.localScale = transform.localScale;
        Rigidbody2D r = thrownObject.GetComponent<Rigidbody2D>();
        thrownObject.GetComponentInChildren<DamageDealer>().
            SetSelfReceiver(GetComponent<DamageReceiver>());

        anim.SetBool("naked", true);
        anim.ResetTrigger("dodge");

        if (r)
            r.velocity = rb.velocity + throwForce * Vector2.down;
    }

    private void RetrieveDrum()
    {
        Vector2 pos = thrownObject.transform.position;
        pos.x += thrownObject.transform.localScale.x * 19 / 32f;

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
        rollStart = Time.time;

        if (transform.localScale.x < 0)
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
        canRoll = true;
        anim.SetBool("can roll", true);
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
