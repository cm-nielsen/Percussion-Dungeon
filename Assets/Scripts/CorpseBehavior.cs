using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseBehavior : MonoBehaviour
{
    public static DynamicMaterialSettings matSet;
    public static PhysicsMaterial2D deadMeat;

    private Material mat;
    private float fade = 0, fadeDelay = 0.75f, fadeLength = 2f;

    // Start is called before the first frame update
    void OnEnable()
    {
        //matSet = (DynamicMaterialSettings)Resources.Load("Scriptable Objects/Corpse Settings");
        //Debug.Log(matSet.a);
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend)
        {
            mat = new Material(matSet.shader);
            mat.SetTexture("_NoiseTex", matSet.tex);
            mat.SetFloat("_Foo", 1);
            rend.material = mat;
        }

        GetComponent<Collider2D>().sharedMaterial = deadMeat;

        fadeDelay = matSet.a;
        fadeLength = matSet.b;
    }

    // Update is called once per frame
    void Update()
    {
        fade += Time.deltaTime;
        if (fade > fadeDelay)
            mat.SetFloat("_Foo", 1 - ((fade - fadeDelay) / fadeLength));

        if (fade > fadeDelay + fadeLength)
        {
            Destroy(mat);
            Destroy(gameObject);
        }
    }
}

[CreateAssetMenu(fileName = "Dynamic Material Settings", menuName = "Scriptable Objects/Create Dynamic Material Settings", order = 1)]
public class DynamicMaterialSettings : ScriptableObject
{
    public Shader shader;
    public Texture2D tex;
    public float a, b;
}
