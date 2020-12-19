using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardsPlayer : MonoBehaviour
{
    private SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
        if (pCon)
            if (pCon.transform.position.x > transform.position.x)
                rend.flipX = true;
            else
                rend.flipX = false;
    }
}
