using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public Material colourMirror;

    private ControlKey pKey;
    private DissolveSceneTransition sceneTransition;
    private Material mat;
    private bool active = false;

    private void Start()
    {
        pKey = GameObject.FindGameObjectWithTag("pControl").
            GetComponent<ControlKey>();
        mat = GetComponent<SpriteRenderer>().sharedMaterial;
        sceneTransition = GetComponentInChildren<DissolveSceneTransition>();
    }

    private void Update()
    {
        if (active && pKey["down"])
        {
            sceneTransition.StartTransition();
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
