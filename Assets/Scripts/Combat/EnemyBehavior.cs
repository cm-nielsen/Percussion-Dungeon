using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(DamageReceiver))]
[RequireComponent(typeof(AudioClipPlayer))]
[RequireComponent(typeof(AnimationMovement))]
public class EnemyBehavior : MonoBehaviour
{
    public enum Type { stationary, roaming, chasing, charging, ranged}
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
        {
            AquireTarget();
            return;
        }

        switch (type)
        {
            case Type.stationary:
                StationaryBehavior();
                break;
            case Type.roaming:

                break;
            case Type.chasing:

                break;
            case Type.charging:
                ChargeBehavior();
                break;
            case Type.ranged:

                break;
        }
        //if (Vector2.Distance(transform.position, target.position) < attackDistance)
        //    anim.SetTrigger("attack");

    }

    private void AquireTarget()
    {
        PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
        if(pCon)
            target = pCon.transform;
    }

    private void StationaryBehavior()
    {
        bool turn = hasTurnAnimation &&
            (rend.flipX && target.transform.position.x > transform.position.x ||
            !rend.flipX && target.transform.position.x < transform.position.x);
        anim.SetBool("turn", turn);

        bool shouldAttack = TargetInSight();// Vector2.Distance(transform.position, target.position) < attackDistance;
        shouldAttack &= Music.beat & !turn;
        anim.SetBool("attack", shouldAttack);
    }

    private void ChargeBehavior()
    {
        anim.SetBool("turn", hasTurnAnimation &&
            (rend.flipX && target.transform.position.x > transform.position.x ||
            !rend.flipX && target.transform.position.x < transform.position.x));

        anim.SetBool("attack", Vector2.Distance(transform.position, target.position) < attackDistance);
    }

    private bool TargetInSight()
    {
        return (rend.flipX && target.transform.position.x + attackDistance > transform.position.x) ||
            (!rend.flipX && target.transform.position.x < attackDistance + transform.position.x);
    }
}
