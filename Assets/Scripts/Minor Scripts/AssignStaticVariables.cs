using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignStaticVariables : MonoBehaviour
{
    public DynamicMaterialSettings corpseMatSettings;
    public PhysicsMaterial2D deadMeat;
    // Start is called before the first frame update
    void Start()
    {
        CorpseBehavior.matSet = corpseMatSettings;
        CorpseBehavior.deadMeat = deadMeat;
    }
}
