using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject baseMenu;
    public GameObject pointer;

    private enum State { closed, main, sub }
    private State state = State.closed;

    private List<GameObject> subMenus;
    private ControlKey input, playerKey;
    private PlayerController pcon;

    private Button back;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<ControlKey>();
        if (!pointer)
            pointer = GameObject.FindObjectOfType<UIPointer>().gameObject;

        playerKey = GameObject.FindGameObjectWithTag("pControl").GetComponent<ControlKey>();

        subMenus = new List<GameObject>();
        foreach (Transform t in baseMenu.transform.parent)
            if (t.name.ToLower().Contains("menu"))
                subMenus.Add(t.gameObject);

        foreach (GameObject g in subMenus)
            g.SetActive(true);
        foreach (RequiresInitialSetup m in GetComponentsInChildren<RequiresInitialSetup>())
            m.Setup();

        foreach (GameObject g in subMenus)
            g.SetActive(false);

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
        state = State.main;
    }

    private void EnterPauseState()
    {
        Time.timeScale = 0;
        playerKey.enabled = false;
    }

    private void ExitPauseState()
    {
        Time.timeScale = 1;
        playerKey.enabled = true;
    }
}

public interface RequiresInitialSetup
{
    void Setup();
}
