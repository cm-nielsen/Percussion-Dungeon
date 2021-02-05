using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Fraction by which to follow in each direction")]
    public Vector2 followMod;

    private GameObject player;

    private void FixedUpdate()
    {
        if (player == null)
            player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        if (!player)
            return;

        Vector2 pos = player.transform.position;
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, pos.x, followMod.x),
            Mathf.Lerp(transform.position.y, pos.y, followMod.y), 0);
    }
}
