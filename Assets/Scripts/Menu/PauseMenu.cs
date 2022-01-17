using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static VisualEffectMenu vfxMenu;
    public static bool active = false;
    private static AudioClip navigateNoiseStatic;

    public GameObject baseMenu;
    public GameObject pointer;
    public AudioClip pauseNoise, navigateNoise, backNoise;

    private enum State { closed, main, sub }
    private State state = State.closed;

    private List<GameObject> subMenus;
    private ControlKey input, playerKey;

    private Button back;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<ControlKey>();
        if (!pointer)
            pointer = FindObjectOfType<UIPointer>().gameObject;

        playerKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();

        subMenus = new List<GameObject>();
        foreach (Transform t in baseMenu.transform.parent)
            if (t.name.ToLower().Contains("menu"))
                subMenus.Add(t.gameObject);

        foreach (GameObject g in subMenus)
            g.SetActive(true);
        foreach (RequiresInitialSetup m in GetComponentsInChildren<RequiresInitialSetup>())
            m.Setup();

        vfxMenu = GetComponentInChildren<VisualEffectMenu>();

        foreach (GameObject g in subMenus)
            g.SetActive(false);

        navigateNoiseStatic = navigateNoise;
    }

    // Update is called once per frame
    void Update()
    {
        if (input["pause"])
        {
            switch (state)
            {
                case State.closed:
                    EnterPauseState();
                    pointer.SetActive(true);
                    baseMenu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                    state = State.main;
                    break;

                case State.main:
                    ExitPauseState();
                    foreach (GameObject g in subMenus)
                        g.SetActive(false);
                    pointer.SetActive(false);
                    baseMenu.SetActive(false);
                    state = State.closed;
                    break;

                case State.sub:
                    ExitPauseState();
                    foreach (GameObject g in subMenus)
                        g.SetActive(false);
                    pointer.SetActive(false);
                    baseMenu.SetActive(false);
                    state = State.closed;
                    break;
            }
        }

        if (input["back"])
        {
            if (state == State.sub)
                back.onClick.Invoke();
        }
    }

    public void EnterSubMenu(GameObject menu)
    {
        state = State.sub;
        menu.SetActive(true);
        for(int i = 0; i < menu.transform.childCount; i++)
        {
            if (menu.transform.GetChild(i).CompareTag("back button"))
                back = menu.transform.GetChild(i).GetComponent<Button>();
        }

        EventSystem.current.SetSelectedGameObject(back.gameObject);
    }

    public void ExitSubMenu()
    {
        AudioClipPlayer.Play(backNoise);
        state = State.main;
    }

    private void EnterPauseState()
    {
        AudioClipPlayer.Play(pauseNoise);
        Time.timeScale = 0;
        active = true;
        playerKey.enabled = false;
    }

    private void ExitPauseState()
    {
        Time.timeScale = 1;
        active = false;
        playerKey.enabled = true;
    }

    public void WipeProgress()
    {
        GameController.WipeProgress();

        GameController gcon = FindObjectOfType<GameController>();
        gcon.SetCurrentWeap(gcon.weaponSet.drumsticks);

        PlayerController p = FindObjectOfType<PlayerController>();
        Vector2 pos = Vector2.zero;
        if (p)
        {
            pos.x = p.transform.position.x;
            pos.y = p.transform.position.y;
            Destroy(p.gameObject);
        }
        else
            return;

        Instantiate(gcon.currentWeaponPrefab, pos, Quaternion.identity);
    }

    public void WipeSettings()
    {
        GameController.WipeSettings();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public static void OnSelectionChange()
    {
        if (navigateNoiseStatic)
            AudioClipPlayer.Play(navigateNoiseStatic);
    }
}

public interface RequiresInitialSetup
{
    void Setup();
}
