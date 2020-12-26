using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointer : MonoBehaviour
{
    public EventSystem eventSystem;

    private Animator anim;
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject g = EventSystem.current.currentSelectedGameObject;//eventSystem.currentSelectedGameObject;
        //if (g)
        //{
        //    MoveObjectToPositionWhenClicked m = g.GetComponent<MoveObjectToPositionWhenClicked>();
        //    if (m)
        //    {
        //        Vector2 v = (Vector3)m.pos + g.transform.position;
        //        transform.position = new Vector2(v.x, transform.position.y);
        //        transform.rotation = m.rot;
        //    }
        //}

        if (pos != transform.position)
        {
            //Debug.Log("YEET");
            pos = transform.position;
            anim.SetTrigger("flip");
        }
    }

    public void UpdatePosition(Vector2 v, Quaternion rot = default)
    {
        transform.position = new Vector2(v.x, transform.position.y);
        transform.rotation = rot;
    }
}
