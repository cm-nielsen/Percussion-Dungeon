using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryAnimation : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend)
        {
            PlayerController p = GameObject.FindObjectOfType<PlayerController>();
            if (p)
                rend.flipX = p.GetComponent<SpriteRenderer>().flipX;
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
