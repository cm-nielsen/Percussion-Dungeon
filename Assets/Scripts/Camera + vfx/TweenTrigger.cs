using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenTrigger : MonoBehaviour
{
    public Tween tween;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
            tween.Play();
    }
}
