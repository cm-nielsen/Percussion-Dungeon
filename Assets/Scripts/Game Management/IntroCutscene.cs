using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    public GameObject boss, player;

    public float delay = 2, sceneTransitionDelay = 4;

    private TweenSet tweens;
    private DissolveSceneTransition sceneTransition;

    // Start is called before the first frame update
    void Start()
    {
        boss.SetActive(false);
        player.SetActive(false);
        sceneTransition = GetComponentInChildren<DissolveSceneTransition>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Destroy(other.gameObject);
        player.SetActive(true);
        StartCoroutine(EnableBoss());
    }

    private IEnumerator EnableBoss()
    {
        yield return new WaitForSeconds(delay);
        boss.SetActive(true);
        yield return new WaitForSeconds(delay);
        sceneTransition.StartTransition();
    }
}
