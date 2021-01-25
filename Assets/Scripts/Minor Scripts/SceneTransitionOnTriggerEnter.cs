using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnTriggerEnter : MonoBehaviour
{
    public string targetScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<PlayerController>())
            return;

        Debug.Log("Portal Reached");
        //SceneManager.LoadScene(targetScene);
    }
}
