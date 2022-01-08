using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InputDisplay : MonoBehaviour
{
    public string[] inputNames;

    public ControlKeyCustomizationMenu controlsMenu;

    private Text display;

    private ControlKey playerControls;
    private PotentialControlNameSet controlNameSet;

    private string text;
    private bool gamepadIsActive;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = GameObject.FindGameObjectWithTag("pControl").
            GetComponent<ControlKey>();
        controlNameSet = controlsMenu.controlSet;
        controlsMenu.UpdateOnChange(UpdateDisplay);
        
        display = GetComponent<Text>();
        text = display.text;

        gamepadIsActive = Gamepad.current != null;

        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        bool b = Gamepad.current != null;
        if (b != gamepadIsActive)
        {
            gamepadIsActive = b;
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
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
}
