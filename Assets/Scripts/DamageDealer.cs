using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public DamageType dType = DamageType.light;
    public float movementValue = 1;


    private List<Collider2D> ignore = new List<Collider2D>();
    private float damageMultiplier = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ignore.Contains(collision))
            return;

        ignore.Add(collision);
        DamageReceiver rec = collision.GetComponent<DamageReceiver>();

        if (!rec)
            return;

        rec.TakeDamage(dType, movementValue * damageMultiplier, transform.position);
    }

    private void StartSwing()
    {
        ignore.Clear();
    }

    private void OnEnable()
    {
        ignore.Clear();
        PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
        if (pCon)
            if (pCon.GetComponent<SpriteRenderer>().flipX)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);

    }
}
