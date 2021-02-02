using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public LayerMask isGround;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isGround == (isGround | ( 1 << collision.gameObject.layer)))
            GetComponent<Animator>().SetBool("hit", true);
    }
}
