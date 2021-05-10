using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnRandom : MonoBehaviour
{
    public float spawnChance = 0.1f;
    public WeightedObject[] pool;

    public void Start()
    {

        GameObject g = null;
        if (pool.Length > 0 && Random.Range(0, 1f) <= spawnChance)
        {
            float total = 0, num, n = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].weight == 0)
                    pool[i].weight = 1;
                total += pool[i].weight;
            }
            num = Random.Range(0, total);
            foreach (WeightedObject w in pool)
            {
                n += w.weight;
                if (num <= n)
                {
                    g = Instantiate(w.obj, transform.position, Quaternion.identity);
                    break;
                }
            }
        }

        Destroy(gameObject);
        Tilemap map = GetComponentInParent<Tilemap>();
        if (map)
        {
            map.SetTile(map.WorldToCell(transform.position), null);
            if (g)
                g.transform.parent = map.transform;
        }
    }
}

[System.Serializable]
public struct WeightedObject
{
    public GameObject obj;
    public float weight;
}
