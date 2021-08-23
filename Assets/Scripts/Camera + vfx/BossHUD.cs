using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHUD : MonoBehaviour
{
    public HealthBarDisplay firstHealthBar, secondHealthBar;
    public Vector2 finalOffset;
    public Tween[] tweens;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Transition(GameObject g)
    {
        foreach (Tween t in tweens)
            t.Play();

        g.GetComponent<Health>().display = secondHealthBar;
    }

    public void OnDeath()
    {

    }
}
