using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToPoint : MonoBehaviour
{
    public GameObject objectToMove;

    public Vector3 pos;
    public Quaternion rot;
    public AudioClip noise;
    // Start is called before the first frame update
    void Start()
    {
        if (!objectToMove)
            objectToMove = GameObject.FindObjectOfType<UIPointer>().gameObject;
    }

    public void ApplyParametersToObject()
    {
        if (!objectToMove)
            return;

        objectToMove.transform.position = transform.position + pos;
        objectToMove.transform.rotation = rot;
        PauseMenu.OnSelectionChange();
    }
}
