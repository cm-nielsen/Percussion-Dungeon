using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    public string targetScene, loadingScene;
    public GameObject transitionObject;

    private ControlKey pKey;
    private bool active = false;

    private void Start()
    {
        pKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();
    }

    private void Update()
    {
        if (active && pKey["down"])
        {
            //if (loadingScene.Length > 1)
            //    SceneManager.LoadSceneAsync(loadingScene);
            //AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
            GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(transitionObject, transform.position, Quaternion.identity);
            active = false;
            
        }
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
