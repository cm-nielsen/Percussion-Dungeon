using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionMenu : MonoBehaviour
{
    public GameObject canvas, iconParent;
    public SpriteRenderer icon;
    public Sprite lockedIcon;
    public List<SelectionItem> options;
    public Text desc, proficiency, level, castas, cost, damageBonus;
    public IconGraph graph;
    public AudioClip swapNoise;

    public float lerpStart, lerpMod, cameraOffset = .5f;

    private Animator anim;
    private GameController gcon;
    private CameraFollow camFollow;
    private AudioClipPlayer sfx;

    private List<PreviousIcon> prevIcons;
    private SelectionItem selected;
    private ControlToggleProxy activate, left, right;
    private Animator castanim;

    private float lerpVal = 0;
    private bool interactable = false, open = false, locked = true;
    // Start is called before the first frame update
    void Start()
    {
        //canvas = GetComponentInChildren<Canvas>().gameObject;
        anim = GetComponent<Animator>();
        gcon = FindObjectOfType<GameController>();
        camFollow = Camera.main.GetComponent<CameraFollow>();
        prevIcons = new List<PreviousIcon>();
        castanim = iconParent.GetComponentInChildren<Animator>();
        canvas.SetActive(false);
        iconParent.SetActive(false);
        sfx = GetComponent<AudioClipPlayer>();

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
            Activate();

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

        locked = !gcon.IsUnlocked(selected.prefab);
        DisplaySelectedAttributes();
    }

    private void Activate()
    {
        if(open && locked)
        {
            if(!gcon.UnlockWeapon(selected.prefab, selected.cost))
            {
                desc.text = "Not enough Castanets";
                return;
            }
            else
            {
                gcon.SetCurrentWeap(selected.prefab);
                locked = false;
                castas.text = GameData.castas + "";
                DisplaySelectedAttributes();
                return;
            }
        }
        ToggleCabinet();
    }

    private void ToggleCabinet()
    {
        open = !open;
        anim.SetBool("open", open);
        if (open)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            SelectCurrentWeapon();
            camFollow.OverrideFollow((Vector2)transform.position + Vector2.up * cameraOffset);
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
        prevIcons.Add(new PreviousIcon(icon, transform, new Vector2(-i, 0) * .75f));

        lerpVal = lerpStart;

        int newIndex = (options.IndexOf(selected) + i) % options.Count;
        if (newIndex < 0)
            newIndex = options.Count + newIndex;
        selected = options[newIndex];

        icon.transform.localPosition = new Vector2(i, 0) * .75f;

        locked = !gcon.IsUnlocked(selected.prefab);
        if (!locked)
            gcon.SetCurrentWeap(selected.prefab);
        DisplaySelectedAttributes();

        castanim.SetTrigger("go");
        if (swapNoise)
            sfx.PlayClip(swapNoise);
    }

    private void DisplaySelectedAttributes()
    {
        float[] ar = gcon.GetLevelInfo();
        cost.enabled = locked;
        cost.text = "" + selected.cost;
        proficiency.enabled = !locked;
        proficiency.text = "+" + ar[0];
        level.text = "Lv: " + ar[2];// + "| +" + ar[3] * 100 + "%";
        if (locked)
        {
            icon.sprite = lockedIcon;
            desc.text = "LOCKED\n" + selected.description;
            damageBonus.text = "+" + (ar[3] * 100) + "%";
        }
        else
        {
            icon.sprite = selected.sprite;
            desc.text = selected.name + "\n" + selected.description;
            damageBonus.text = "+" + ((ar[1] + ar[3]) * 100) + "%";
        }
        if (graph.isActiveAndEnabled)
            graph.Graph(selected.stats);
    }

    private void RunSwap()
    {
        if (open)
        {
            prevIcons.RemoveAll(x => x.rend == null);
            foreach (PreviousIcon i in prevIcons)
                i.Update(lerpVal);
            
            icon.transform.localPosition =
                Vector2.Lerp(icon.transform.localPosition, Vector2.zero, lerpVal);
            lerpVal *= lerpMod;
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

    private void OnDoorOpen()
    {
        iconParent.SetActive(true);
        canvas.SetActive(true);
        castas.text = GameData.castas + "";
        DisplaySelectedAttributes();
    }

    private void OnDoorClose()
    {
        prevIcons.RemoveAll(x => x.rend == null);
        foreach (PreviousIcon i in prevIcons)
            Destroy(i.rend.gameObject);
        prevIcons.Clear();

        graph.Clear();
        iconParent.SetActive(false);
        canvas.SetActive(false);
    }

    private class PreviousIcon
    {
        public SpriteRenderer rend;
        private Vector2 lerpGoal;

        public PreviousIcon(SpriteRenderer icon, Transform t, Vector2 v)
        {
            rend = Instantiate(icon, t);
            rend.name = "Previous Icon";
            rend.transform.localPosition = Vector2.zero;
            lerpGoal = v;
        }

        public void Update(float lerpVal)
        {
            rend.transform.localPosition =
                    Vector2.Lerp(rend.transform.localPosition, lerpGoal, lerpVal);
            if (Vector2.Distance(rend.transform.localPosition, lerpGoal) < 0.1f)
                Destroy(rend.gameObject);
        }
    }
}

[System.Serializable]
public class SelectionItem
{
    public string name, description;
    public GameObject prefab;
    public Sprite sprite;
    public int cost = 1;

    public int[] stats = new int[3]; //speed, power, usability
}