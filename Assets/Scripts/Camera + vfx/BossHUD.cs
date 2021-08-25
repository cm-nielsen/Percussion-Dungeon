using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHUD : MonoBehaviour
{
    public HealthBarDisplay secondHealthBar;
    //public Tween[] tweens;

    public void Transition(GameObject g)
    {
        //foreach (Tween t in tweens)
        //    t.Play();

        g.GetComponent<Health>().display = secondHealthBar;
    }

    public void OnDeath()
    {

    }
}
