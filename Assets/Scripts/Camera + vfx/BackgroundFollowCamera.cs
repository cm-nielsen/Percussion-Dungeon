using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    public Vector2 tileSize;
    public float parallaxRatio;

    private Transform cam;
    private Vector2 stepPos, parallaxOffset, paraOffOff;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>().transform;
        stepPos = cam.position;
        parallaxOffset = stepPos * parallaxRatio;
    }

    // Update is called once per frame
    void Update()
    {
        float dif = stepPos.x - cam.position.x;
        while (dif < -tileSize.x)
        {
            stepPos.x += tileSize.x;
            dif = stepPos.x - cam.position.x;
        }
        while (dif > tileSize.x)
        {
            stepPos.x -= tileSize.x;
            dif = stepPos.x - cam.position.x;
        }

        dif = stepPos.y - cam.position.y;
        while (dif < -tileSize.y)
        {
            stepPos.y += tileSize.y;
            dif = stepPos.y - cam.position.y;
        }
        while (dif > tileSize.y)
        {
            stepPos.y -= tileSize.y;
            dif = stepPos.y - cam.position.y;
        }

        parallaxOffset = cam.position * parallaxRatio;
        transform.position = stepPos + parallaxOffset;
    }

    private float Diff(float a, float b)
    {
        return (a - b);
    }
}
