using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseBehavior : MonoBehaviour
{
    public static DynamicMaterialSettings matSet;
    public static PhysicsMaterial2D deadMeat;
    public static AudioClip thudSound;

    private AudioClipPlayer audioPlayer;
    private float velocityThreshold = .5f;

    private Material mat;
    private float fade = 0, fadeDelay = 0.75f, fadeLength = 2f;

    // Start is called before the first frame update
    void OnEnable()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend)
        {
            mat = new Material(matSet.shader);
            mat.SetTexture("_NoiseTex", matSet.tex);
            mat.SetFloat("_Foo", 1);
            rend.material = mat;

            mat.SetColor("_MainCol", matSet.refMat.GetColor("_MainCol"));
            mat.SetColor("_MonoCol", matSet.refMat.GetColor("_MonoCol"));
        }

        GetComponent<Collider2D>().sharedMaterial = deadMeat;

        fadeDelay = matSet.a;
        fadeLength = matSet.b;

        audioPlayer = gameObject.AddComponent<AudioClipPlayer>();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float speed = rb.velocity.magnitude;
        if (rb && speed > velocityThreshold)
            audioPlayer.PlayClip(thudSound, speed / 4);
    }
}

//[CreateAssetMenu(fileName = "Dynamic Material Settings", menuName = "Scriptable Objects/Create Dynamic Material Settings", order = 1)]
[System.Serializable]
public struct DynamicMaterialSettings
{
    public Shader shader;
    public Material refMat;
    public Texture2D tex;
    public float a, b;
}
