using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Object to follow")]
    public GameObject player;
    [Header("Fraction by which to follow in each direction")]
    [Range(0, 1)]
    public float xMod = 0.075f;
    [Range(0, 1)]
    public float yMod = 0.1f;
    [Header("How much velocity to apply as an position offset")]
    [Range(0, 1)]
    public float velMod = 0.35f;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindObjectOfType<PlayerController>().gameObject;
    }

    /// <summary>
    /// sets the camera position a fraction of the distance away from centered on the player, 
    /// a fraction of velocity is used as a predictive offset fro where the camera should aim for
    /// </summary>
    void FixedUpdate()
    {

        Vector3 camPos = transform.position, pPos = player.transform.position, velocity = player.GetComponent<Rigidbody2D>().velocity;
        pPos += velocity * velMod;

        transform.position = new Vector3
            (camPos.x - (camPos.x - pPos.x) * xMod,
            camPos.y - (camPos.y - pPos.y) * yMod, -10);
    }
}
