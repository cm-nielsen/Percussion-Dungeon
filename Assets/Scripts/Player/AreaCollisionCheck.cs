using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollisionCheck : MonoBehaviour
{
    public LayerMask mask;

    private SpriteRenderer rend;
    private Vector2 offset;

    private void Start()
    {
        offset = transform.localPosition;
        rend = GetComponentInParent<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    public bool Check()
    {
        Vector2 pos = transform.position;
        Vector2 size = transform.localScale;
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        if (rend && rend.flipX)
            pos.x -= offset.x * 2;
        return Physics2D.OverlapBox(pos, size, 0, 1 << 12);
    }
}
