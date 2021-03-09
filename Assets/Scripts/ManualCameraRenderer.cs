using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ManualCameraRenderer : MonoBehaviour
{
    public int fps = 16;
    float elapsed, threshhold;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
        threshhold = 1f / fps;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > threshhold)
        {
            elapsed -= threshhold;
            cam.Render();
        }
        //Debug.Log("YEET");
        //cam.Render();
    }
}