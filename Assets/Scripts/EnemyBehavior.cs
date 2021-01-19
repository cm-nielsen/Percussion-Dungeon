using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public enum Type { stationary, roaming, chasing, ranged}
    public Type type;

    public float attackDistance;

    private Animator anim;
    private SpriteRenderer rend;
    private Transform target;

    private bool hasTurnAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        foreach (AnimatorControllerParameter p in anim.parameters)
            if (p.name == "turn")
                hasTurnAnimation = true;
        AquireTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
            return;

        anim.SetBool("turn", hasTurnAnimation &&
            (rend.flipX && target.transform.position.x > transform.position.x ||
            !rend.flipX && target.transform.position.x < transform.position.x));

        anim.SetBool("attack", Vector2.Distance(transform.position, target.position) < attackDistance);
        //if (Vector2.Distance(transform.position, target.position) < attackDistance)
        //    anim.SetTrigger("attack");

    }

    private void AquireTarget()
    {
        target = GameObject.FindObjectOfType<PlayerController>().transform;
    }
}
