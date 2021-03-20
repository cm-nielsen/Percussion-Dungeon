using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnRandom : MonoBehaviour
{
    public float spawnChance = 0.1f;
    public GameObject[] pool;

    public void Start()
    {
        if (pool.Length > 0 && Random.Range(0, 1f) <= spawnChance)
        {
            Instantiate(pool[Random.Range(0, pool.Length)],
                transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        Tilemap map = GetComponentInParent<Tilemap>();
        if (map)
            map.SetTile(map.WorldToCell(transform.position), null);

    }
}
