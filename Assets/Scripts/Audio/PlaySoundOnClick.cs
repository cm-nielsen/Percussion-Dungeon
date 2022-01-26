using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnClick : MonoBehaviour
{
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0)
            return;

        if (Mouse.current.leftButton.isPressed)
        {
            source.Play();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Keyboard.current.escapeKey.isPressed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
