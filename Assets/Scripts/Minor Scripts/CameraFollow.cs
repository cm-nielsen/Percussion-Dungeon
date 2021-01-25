using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Object to follow")]
    public GameObject player;
    [Header("Fraction by which to follow in each direction")]
    public Vector2 followMod;

    private float maxYDist;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindObjectOfType<PlayerController>().gameObject;

        maxYDist = GetComponent<Camera>().orthographicSize * 2 / 3;
    }

    /// <summary>
    /// sets the camera position a fraction of the distance away from centered on the player, 
    /// a fraction of velocity is used as a predictive offset fro where the camera should aim for
    /// </summary>
    void FixedUpdate()
    {
        if (!player)
            return;

        Vector2 pos = player.transform.position;
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, pos.x, followMod.x),
            Mathf.Lerp(transform.position.y, pos.y, followMod.y), -10);

        if (transform.position.y > pos.y + maxYDist)
            transform.position = new Vector3(transform.position.x,
                pos.y + maxYDist, -10);
        if (transform.position.y < pos.y - maxYDist)
            transform.position = new Vector3(transform.position.x,
                pos.y - maxYDist, -10);
    }
}
