using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject player = GameObject.FindObjectOfType<GameController>().currentWeaponPrefab;
        player = Instantiate(player);
        player.transform.position = transform.position;
    }
    private void MovePlayer()
    {
        Transform player = GameObject.FindObjectOfType<PlayerController>().transform;
        player.position = transform.position;
    }
}
