using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    public bool loadAsynchronously;

    public void LoadScene()
    {
        if (loadAsynchronously)
            SceneManager.LoadSceneAsync(targetScene);
        else
            SceneManager.LoadScene(targetScene);
    }

    private void ToggleText()
    {
        GetComponentInChildren<Text>().enabled = true;
    }
}
