using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject loadingScreen;
    public Vector2 loadingScreenSpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(loadingScreen, 
            transform.position + (Vector3)loadingScreenSpawnPos,
            Quaternion.identity);
    }
}
