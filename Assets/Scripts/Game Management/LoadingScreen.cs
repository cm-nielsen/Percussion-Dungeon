using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static bool loaded = false;
    // Start is called before the first frame update
    void Start()
    {
        loaded = false;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position + Vector3.forward * 10;

        if (loaded)
            Destroy(gameObject);
    }
}
