using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCastanetCount : MonoBehaviour
{
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "You have\n[" + GameData.castas + "]\nCastanets," +
            "\nI promise they\nwill be useful,\neventually";
    }
}
