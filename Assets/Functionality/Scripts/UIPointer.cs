using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointer : MonoBehaviour
{
    private Animator anim;
    private RectTransform tr;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePosition(Vector2 v, Quaternion rot = default)
    {
        transform.position = new Vector2(v.x, transform.position.y);
        transform.rotation = rot;
        anim.SetTrigger("flip");
    }

    public void UpdatePosition(float v, float y)
    {
        //tr.position = v;
        anim.SetTrigger("flip");
    }
}
