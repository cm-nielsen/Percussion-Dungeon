using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject baseMenu;
    public GameObject[] subMenus;
    public GameObject pointer;
    public ControlKey playerKey;

    private enum State { closed, main, sub }
    private State state = State.closed;

    private ControlKey input;
    private PlayerController pcon;
    //private EnemyScript[] ens;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<ControlKey>();
        if (!pointer)
            pointer = GameObject.FindObjectOfType<UIPointer>().gameObject;
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
            switch (state)
            {
                case State.closed:

                    break;

                case State.main:
                    //ExitPauseState();
                    //foreach (GameObject g in subMenus)
                    //    g.SetActive(false);
                    //pointer.SetActive(false);
                    //baseMenu.SetActive(false);
                    //state = State.closed;
                    break;

                case State.sub:
                    pointer.SetActive(true);
                    baseMenu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                    state = State.main;
                    foreach (GameObject g in subMenus)
                        g.SetActive(false);
                    break;
            }
        }
    }

    public void EnterSubMenu()
    {
        state = State.sub;
    }

    public void ExitSubMenu()
    {
        state = State.main;
    }

    private void EnterPauseState()
    {
        Time.timeScale = 0;
        //pcon = GameObject.FindObjectOfType<PlayerController>();
        //if (pcon)
        //    pcon.enabled = false;
        playerKey.enabled = false;
        //playerKey["dodge"] = false;
    }

    private void ExitPauseState()
    {
        Time.timeScale = 1;
        //pcon.enabled = true;
        playerKey.enabled = true;
    }
}
