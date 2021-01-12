using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public DamageType dType = DamageType.light;
    public Material damageTextMaterial;
    public GameObject damageTextPrefab;
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

        if (damageTextPrefab)
        {
            GameObject g = Instantiate(damageTextPrefab, collision.transform.position, Quaternion.identity);
            g.GetComponent<LerpFromPoint>().Initiate(damageTextMaterial, movementValue * damageMultiplier);
        }
    }

    private void StartSwing()
    {
        ignore.Clear();
    }

    private void OnEnable()
    {
        ignore.Clear();
        SpriteRenderer parentSprite = GetComponentInParent<SpriteRenderer>();
        if (parentSprite)
            if (parentSprite.flipX)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);

    }
}
