using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public enum Type { stationary, roaming, chasing, ranged}
    public Type type;

    public float attackDistance;

    private Animator anim;
    private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        AquireTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) < attackDistance)
            anim.SetTrigger("attack");
    }

    private void AquireTarget()
    {
        target = GameObject.FindObjectOfType<PlayerController>().transform;
    }
}
