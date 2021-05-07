using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryAnimation : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(name);
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if(rend)
            rend.flipX = GameObject.FindObjectOfType<PlayerController>().GetComponent<SpriteRenderer>().flipX;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
