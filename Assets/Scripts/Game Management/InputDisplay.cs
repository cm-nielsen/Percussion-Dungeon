using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InputDisplay : MonoBehaviour
{
    public string[] inputNames;

    //public ControlKeyCustomizationMenu controlsMenu;

    public bool appearOnTrigger = false;

    private Text display;

    private ControlKey playerControls;
    private ControlKeyCustomizationMenu controlsMenu;
    private PotentialControlNameSet controlNameSet;

    private string text;
    private bool gamepadIsActive;

    // Start is called before the first frame update
    void Start()
    {
        gamepadIsActive = Gamepad.current != null;

        playerControls = GameObject.FindGameObjectWithTag("pControl").
            GetComponent<ControlKey>();
        FetchControlsMenu();

        if (!display)
        {
            display = GetComponent<Text>();
            text = display.text;
        }

        //UpdateDisplay();
        if (appearOnTrigger)
            display.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlsMenu)
            FetchControlsMenu();

        bool b = Gamepad.current != null;
        if (b != gamepadIsActive)
        {
            gamepadIsActive = b;
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        if (!controlsMenu)
            return;
        if (!display)
        {
            display = GetComponent<Text>();
            text = display.text;
        }

        System.Func<string, string> fetchName = str =>
        {
            if (gamepadIsActive)
                return playerControls.GetUnit(str).gamePadButtons[0];
            else
                return playerControls.GetUnit(str).keyCodes[0];
        };

        string displayText = text;
        for( int i = 0; i < inputNames.Length; i++)
            displayText = displayText.Replace(
                $"${i}", fetchName(inputNames[i]));

        display.text = displayText;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!appearOnTrigger || !collision.CompareTag("Player"))
            return;

        display.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!appearOnTrigger || !collision.CompareTag("Player"))
            return;

        display.enabled = false;
    }

    private void FetchControlsMenu()
    {
        controlsMenu = ControlKeyCustomizationMenu.instance;
        if (controlsMenu)
        {
            controlNameSet = controlsMenu.controlSet;
            controlsMenu.UpdateOnChange(UpdateDisplay);
            UpdateDisplay();
        }
    }
}
