using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class AssignStaticVariables : MonoBehaviour
{
    public int frameRate = 16;

    public DynamicMaterialSettings corpseMatSettings;
    public PhysicsMaterial2D deadMeat;
    public AudioClip corpseSound;

    public TileBase platformTile, jarTile;

    public AudioSourceSettings audioSettings;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 16;

        CorpseBehavior.matSet = corpseMatSettings;
        CorpseBehavior.deadMeat = deadMeat;
        CorpseBehavior.thudSound = corpseSound;

        Room.platformTile = platformTile;
        Room.jarTile = jarTile;

        AudioClipPlayer.settings = audioSettings;
    }
}
