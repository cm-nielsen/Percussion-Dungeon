using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    public string targetScene, loadingScene;
    public GameObject transitionObject;
    public Material colourMirror;

    private ControlKey pKey;
    private Material mat;
    private bool active = false;

    private void Start()
    {
        pKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();
        mat = GetComponent<SpriteRenderer>().sharedMaterial;
    }

    private void Update()
    {
        if (active && pKey["down"])
        {
            GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(transitionObject, transform.position, Quaternion.identity);
            active = false;
        }

        mat.SetColor("_MainCol", colourMirror.GetColor("_MainCol"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        active = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        active = false;
    }
}
