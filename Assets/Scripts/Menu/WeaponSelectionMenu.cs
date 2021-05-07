using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionMenu : MonoBehaviour
{
    public GameObject canvas;
    public SpriteRenderer back, icon, prevIcon;
    public List<SelectionItem> options;
    public Text desc, level, overallLevel;

    public float lerpStart, lerpMod;

    private Animator anim;
    private GameController gcon;
    private CameraFollow camFollow;

    private SelectionItem selected;
    private Vector2 lerpGoal;
    private ControlToggleProxy activate, left, right;

    private float lerpVal = 0;
    private bool interactable = false, open = false;
    // Start is called before the first frame update
    void Start()
    {
        //canvas = GetComponentInChildren<Canvas>().gameObject;
        anim = GetComponent<Animator>();
        gcon = GameObject.FindObjectOfType<GameController>();
        camFollow = Camera.main.GetComponent<CameraFollow>();
        canvas.SetActive(false);

        activate = new ControlToggleProxy();
        left = new ControlToggleProxy();
        right = new ControlToggleProxy();
        ControlKey pKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();
        activate.Setup(pKey, "down");
        left.Setup(pKey, "left");
        right.Setup(pKey, "right");

        SelectCurrentWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        activate.Update();
        left.Update();
        right.Update();

        if (interactable && activate.val)
            ToggleCabinet();

        if (open)
        {
            if (left.val)
                SwitchSelection(1);
            else if (right.val)
                SwitchSelection(-1);
        }

        RunSwap();
    }

    private void SelectCurrentWeapon()
    {
        selected = options[0];
        foreach (SelectionItem i in options)
            if (i.prefab == gcon.currentWeaponPrefab)
                selected = i;
        DisplaySelectedAttributes();
    }

    private void ToggleCabinet()
    {
        open = !open;
        anim.SetBool("open", open);
        canvas.SetActive(open);
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
    }

    private void SwitchSelection(int i)
    {
        prevIcon.enabled = true;
        prevIcon.transform.localPosition = Vector2.zero;
        prevIcon.sprite = selected.sprite;
        lerpGoal = new Vector2(-i, 0) * .75f;
        lerpVal = lerpStart;

        int newIndex = (options.IndexOf(selected) + i) % options.Count;
        if (newIndex < 0)
            newIndex = options.Count + newIndex;
        selected = options[newIndex];

        icon.transform.localPosition = new Vector2(i, 0) * .75f;
        DisplaySelectedAttributes();

        gcon.SetCurrentWeap(selected.prefab);
    }

    private void DisplaySelectedAttributes()
    {
        icon.sprite = selected.sprite;
        desc.text = selected.description;
    }

    private void RunSwap()
    {
        if (open && prevIcon.enabled)
        {
            prevIcon.transform.localPosition =
                Vector2.Lerp(prevIcon.transform.localPosition, lerpGoal, lerpVal);
            icon.transform.localPosition =
                Vector2.Lerp(icon.transform.localPosition, Vector2.zero, lerpVal);
            lerpVal *= lerpMod;

            if (Vector2.Distance(icon.transform.localPosition, Vector2.zero) < 0.01f)
            {
                prevIcon.enabled = false;
                icon.transform.localPosition = Vector2.zero;
            }
        }
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

    private void OnDoorOpen() { back.enabled = icon.enabled = true; }

    private void OnDoorClose() { back.enabled = icon.enabled = prevIcon.enabled = false; }
}

[System.Serializable]
public class SelectionItem
{
    public string name, description;
    public GameObject prefab;
    public Sprite sprite;

    public int[] stats = new int[3]; //speed, power, usability
}
