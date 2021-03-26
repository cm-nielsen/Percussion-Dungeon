using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static bool loaded = false;

    private CameraFollow cam;
    // Start is called before the first frame update
    void Start()
    {
        loaded = false;
        DontDestroyOnLoad(gameObject);
        cam = Camera.main.GetComponent<CameraFollow>();
        cam.OverrideFollow(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(!cam)
        {
            cam = Camera.main.GetComponent<CameraFollow>();
            cam.OverrideFollow(transform.position);
        }
        //Camera.main.transform.position = transform.position - Vector3.forward * 10;

        if (loaded)
        {
            cam.ResetFollowOverride();
            Destroy(gameObject);
        }
    }
}
