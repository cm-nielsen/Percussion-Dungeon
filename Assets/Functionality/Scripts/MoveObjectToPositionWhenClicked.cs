using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToPositionWhenClicked : MonoBehaviour
{
    public GameObject objectToMove;

    public Vector3 pos;
    public Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyParametersToObject()
    {
        objectToMove.transform.position = transform.position + pos;
        objectToMove.transform.rotation = rot;
    }
}
