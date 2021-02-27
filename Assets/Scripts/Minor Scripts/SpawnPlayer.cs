using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindObjectOfType<GameController>().currentWeaponPrefab;
        player = Instantiate(player);
        //Tilemap map = transform.parent.parent.GetComponent<Tilemap>();
        //player.transform.position = map.LocalToWorld(transform.position);
        player.transform.position = transform.position;
    }
    private void MovePlayer()
    {
        Transform player = GameObject.FindObjectOfType<PlayerController>().transform;
        player.position = transform.position;
    }
}
