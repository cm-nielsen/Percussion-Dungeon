using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static bool loaded = false;

    public Vector2 position;
    public string targetScene;

    private CameraFollow cam;
    private bool trigger = true;
    // Start is called before the first frame update
    void Start()
    {
        loaded = false;
        DontDestroyOnLoad(gameObject);
        cam = Camera.main.GetComponent<CameraFollow>();
        cam.OverrideFollow(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger && Vector2.Distance(transform.position, cam.transform.position) < .1)
        {
            SceneManager.LoadScene(targetScene);
            trigger = false;
        }

        if (loaded)
        {
            cam.ResetFollowOverride();
            Destroy(gameObject);
        }
    }

    public void OnLoad(CameraFollow cam)
    {
        transform.position = position;
        cam.OverrideFollow(transform.position, true);
        this.cam = cam;
    }
}
