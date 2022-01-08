using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DissolveSceneTransition: MonoBehaviour
{
    public string targetScene;
    [Header("controls blend effect - must start at 0 and end at 1")]
    public AnimationCurve curve;
    public float transitionTime = 1;
    public bool loadAsynchronously, fadeIn;

    private Material dissolveMaterial;
    private float blend = 0, blendTarget = 1;
    private float startTime;
    private bool active = false;

    void Start()
    {
        dissolveMaterial = GetComponent<SpriteRenderer>().material;
        if (fadeIn)
        {
            blend = 1;
            blendTarget = 0;
            StartTransition();
        }
        dissolveMaterial.SetFloat("_Foo", blend);
    }

    public void LoadScene()
    {
        if (loadAsynchronously)
            SceneManager.LoadSceneAsync(targetScene);
        else
            SceneManager.LoadScene(targetScene);
    }

    public void StartTransition()
    {
        active = true;
        startTime = Time.time;
    }

    void Update()
    {
        if (active)
        {
            blend = Mathf.Lerp(blend, blendTarget,
                curve.Evaluate(Time.time - startTime));
            dissolveMaterial.SetFloat("_Foo", blend);
            if (blend == blendTarget)
                if (fadeIn)
                    Destroy(gameObject);
                else
                    LoadScene();
        }
    }
}
