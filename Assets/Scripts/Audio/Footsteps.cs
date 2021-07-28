using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : AudioClipPlayer
{
    private static new Footsteps instance;
    private static int n;

    public AudioClip[] footsteps;


    private void Start()
    {
        if (isStatic)
            if (instance == null)
            {
                instance = this;
                n = footsteps.Length;
            }
            else
                Destroy(this);
    }

    private void Step()
    {
        if (!instance)
            return;
        int r = Random.Range(0, n);
        PlayClip(instance.footsteps[r]);
    }
}
