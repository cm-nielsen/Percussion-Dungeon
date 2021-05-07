using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionMenu : MonoBehaviour
{
    public GameObject canvas;
    public SpriteRenderer back, icon;

    private Animator anim;
    private GameController gcon;
    private ControlKey pKey;
    private CameraFollow camFollow;
 
    private bool interactable = false, open = false, input = false, inputToggle = true;
    // Start is called before the first frame update
    void Start()
    {
        //canvas = GetComponentInChildren<Canvas>().gameObject;
        anim = GetComponent<Animator>();
        gcon = GameObject.FindObjectOfType<GameController>();
        pKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();
        camFollow = Camera.main.GetComponent<CameraFollow>();
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputToggle)
            inputToggle = pKey["down"];
        else
            input = pKey["down"];

        if (interactable && input)
            ToggleCabinet();

    }

    public void ChangeWeapon(GameObject weapPrefab)
    {
        Debug.Log(weapPrefab);
        canvas.SetActive(false);
        camFollow.ResetFollowOverride();
        //pKey.enabled = true;
        //Time.timeScale = 1;
        gcon.SetCurrentWeap(weapPrefab);
        //Destroy(GameObject.FindGameObjectWithTag("Player"));
        Instantiate(weapPrefab, transform.position, Quaternion.identity);
    }

    private void ToggleCabinet()
    {
        input = false;
        inputToggle = true;
        open = !open;
        if (open)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            camFollow.OverrideFollow((Vector2)transform.position + Vector2.up / 2);
        }
        else
        {
            camFollow.ResetFollowOverride();
            Instantiate(gcon.currentWeaponPrefab,
                (Vector2)transform.position + Vector2.down * .8f, Quaternion.identity);
        }
        anim.SetBool("open", open);
        canvas.SetActive(open);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        interactable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (open || !collision.CompareTag("Player"))
            return;
        interactable = false;
    }

    private void OnDoorOpen()
    {
        back.enabled = icon.enabled = true;
    }

    private void OnDoorClose()
    {
        back.enabled = icon.enabled = false;
    }
}
