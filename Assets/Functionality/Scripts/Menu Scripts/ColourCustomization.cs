using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourCustomization : MonoBehaviour
{
    [Header("Player Colours")]
    public Color bodyCol;
    public Color weaponCol;
    [Header("Other Colours")]
    public Color enemyCol;
    public Color platformCol, backgroundCol, objectCol, interactableCol;

    [Header("Materials")]
    public Material playerMat;
    public Material enemyMat, platformMat,
        backgroundMat, objectMat, interactableMat;

    [Header("Sliders")]
    public Slider mainR;
    public Slider mainG, mainB, monoR, monoG, monoB;
    private Material currentMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCurrentMat(Material mat)
    {
        currentMat = mat;
        //Color main = mat.GetColor("_MainCol");
        //Color mono = mat.GetColor("_MonoCol");

        //Debug.Log(main.g);
        //mainR.value = main.r;
        //mainG.value = main.g;
        //mainB.value = main.b;

        //monoR.value = mono.r;
        //monoG.value = mono.g;
        //monoB.value = mono.b;
    }
}
