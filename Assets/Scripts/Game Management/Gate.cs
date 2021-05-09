using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public static bool loaded = false;

    public SpriteRenderer loadingBar, loadingIcon;

    private LevelGenerator generator;
    private float maxBarWidth;

    // Start is called before the first frame update
    void Start()
    {
        loaded = false;
        if (loadingBar)
        {
            loadingBar.drawMode = SpriteDrawMode.Tiled;
            maxBarWidth = loadingBar.size.x;
            loadingBar.size = new Vector2(0, loadingBar.size.y);
        }
        generator = GameObject.FindObjectOfType<LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (generator && loadingBar)
            loadingBar.size = new Vector2(Mathf.Pow(generator.progress, 2) * maxBarWidth, loadingBar.size.y);

        if (loaded)
        {
            if(loadingIcon) loadingIcon.GetComponent<Animator>().SetTrigger("go");
            if (loadingBar)
            {
                loadingBar.enabled = false;
                foreach (SpriteRenderer r in loadingBar.GetComponentsInChildren<SpriteRenderer>())
                    r.enabled = false;
            }

            foreach (Animator a in GetComponentsInChildren<Animator>())
                a.SetTrigger("go");

            GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
        }
    }
}
