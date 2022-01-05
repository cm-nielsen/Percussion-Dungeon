using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays footstep sound random footstep sound from animation events
/// events are called with Step() on local instances,
/// while the static instance (on the Audio prefab) stores the actual sounds
/// </summary>
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
