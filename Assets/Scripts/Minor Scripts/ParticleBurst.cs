using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBurst : MonoBehaviour
{
    public GameObject itemToSpawn;
    public int numberToSpawn;

    private ParticleSystem pSystem;
    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        if(itemToSpawn != null)
        {
            for(int i = 0; i < numberToSpawn; i++)
            {
                Instantiate(itemToSpawn, transform.position, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!pSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
