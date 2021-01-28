using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class AssignStaticVariables : MonoBehaviour
{
    public DynamicMaterialSettings corpseMatSettings;
    public PhysicsMaterial2D deadMeat;
    public TileBase platformTile, jarTile;
    // Start is called before the first frame update
    void Start()
    {
        CorpseBehavior.matSet = corpseMatSettings;
        CorpseBehavior.deadMeat = deadMeat;
        Room.platformTile = platformTile;
        Room.jarTile = jarTile;
    }
}
