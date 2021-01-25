using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerToPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {
        Transform player = GameObject.FindObjectOfType<PlayerController>().transform;
        player.position = transform.position;
    }
}
