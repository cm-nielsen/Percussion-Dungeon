using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColourPresetMenu : MonoBehaviour
{
    public List<ColourPreset> presets;

    public Shader colourShader;
    public GameObject colourPreview, colourView, presetButton;
    private Image[] previewImages, currentImages;
    // Start is called before the first frame update
    void Start()
    {
        currentImages = colourView.GetComponentsInChildren<Image>();
        previewImages = colourPreview.GetComponentsInChildren<Image>();
        foreach (Image i in previewImages)
            i.material = new Material(colourShader);

        MakePresetOptions();

        UpdatePreview(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MakePresetOptions()
    {
        for(int i = 1; i < presets.Count; i++)
        {
            GameObject g = Instantiate(presetButton, transform);
            g.AddComponent<ColourPresetButton>().Initialize(i, UpdatePreview, ApplyPreset, presets[i].name);
        }

        presetButton.AddComponent<ColourPresetButton>().Initialize(0, UpdatePreview, ApplyPreset, presets[0].name);
    }

    public void UpdatePreview(int index)
    {
        if (index >= presets.Count)
            return;

        int i = 0;
        foreach (ColourPair p in presets[index])
        {
            previewImages[i].material.SetColor("_MainCol", p.main);
            previewImages[i++].material.SetColor("_MonoCol", p.mono);
        }
    }

    public void ApplyPreset(int index)
    {
        if (index >= presets.Count)
            return;

        int i = 0;
        foreach (ColourPair p in presets[index])
        {
            currentImages[i].material.SetColor("_MainCol", p.main);
            currentImages[i++].material.SetColor("_MonoCol", p.mono);
        }
    }
}
