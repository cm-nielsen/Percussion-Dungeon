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
        if (dif < -tileSize.x)
            stepPos.x += tileSize.x;
        else if (dif > tileSize.x)
            stepPos.x -= tileSize.x;

        dif = stepPos.y - cam.position.y;
        if (dif < -tileSize.y)
            stepPos.y += tileSize.y;
        if (dif > tileSize.y)
            stepPos.y -= tileSize.y;

        parallaxOffset = cam.position * parallaxRatio;
        transform.position = stepPos + parallaxOffset;
    }

    private float Diff(float a, float b)
    {
        return (a - b);
    }
}
