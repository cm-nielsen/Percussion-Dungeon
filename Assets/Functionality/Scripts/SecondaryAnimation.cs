using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryAnimation : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().flipX = GameObject.FindObjectOfType<PlayerController>().GetComponent<SpriteRenderer>().flipX;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
