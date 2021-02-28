using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionMenu : MonoBehaviour
{
    public GameObject canvas;

    private GameController gcon;
    private ControlKey pKey;

    private int timer;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        //canvas = GetComponentInChildren<Canvas>().gameObject;
        gcon = GameObject.FindObjectOfType<GameController>();
        pKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (active && pKey["down"])
        {
            canvas.SetActive(true);
            pKey["down"] = false;
            pKey.enabled = false;
            Time.timeScale = 0;
        }
        if (!active)
        {
            timer++;
        }
    }

    public void ChangeWeapon(GameObject weapPrefab)
    {
        pKey["down"] = false;
        Debug.Log(pKey["down"]);
        active = false;
        timer = 0;
        canvas.SetActive(false);
        pKey.enabled = true;
        Time.timeScale = 1;
        gcon.SetCurrentWeap(weapPrefab);
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Instantiate(weapPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timer < 8 || !collision.CompareTag("Player"))
            return;
        active = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (timer < 8 || !collision.CompareTag("Player"))
            return;
        active = false;
    }
}
