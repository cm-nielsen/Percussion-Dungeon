using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [Header("Fraction by which to follow in each direction")]
    public Vector2 followMod;

    public GameObject toFollow;
    public bool maintainOffset = false, followPlayer = true;

    private Vector2 offset;

    private void Start()
    {
        if (followPlayer)
        {
            PlayerController pcon = GameObject.FindObjectOfType<PlayerController>();
            if (pcon)
                toFollow = pcon.gameObject;
        }
        offset = maintainOffset?
            (Vector2)(transform.position - toFollow.transform.position) : Vector2.zero;
    }

    private void Update()
    {
        if (!toFollow)
        {
            if (followPlayer)
            {
                PlayerController pcon = GameObject.FindObjectOfType<PlayerController>();
                if (pcon)
                    toFollow = pcon.gameObject;
            }
            return;
        }

        Vector2 pos = (Vector2)toFollow.transform.position + offset;
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, pos.x, followMod.x),
            Mathf.Lerp(transform.position.y, pos.y, followMod.y), 0);
    }
}
