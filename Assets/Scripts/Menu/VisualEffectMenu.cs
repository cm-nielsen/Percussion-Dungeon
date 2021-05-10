using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffectMenu : MonoBehaviour, RequiresInitialSetup
{
    public List<VisualEffectBoolean> toggleEffects;
    public List<VisualEffectSlider> sliderEffects;
    public List<GameObjectToggle> volumeEffects;
    public Slider camShakeSlider, shakeFreqSlider;
    public Sprite tBox, fBox;

    private List<VisualEffect> effects = new List<VisualEffect>();

    public void Setup()
    {
        VisualEffect.tBox = tBox;
        VisualEffect.fBox = fBox;
        foreach (VisualEffect e in toggleEffects)
            effects.Add(e);
        foreach (VisualEffect e in sliderEffects)
            effects.Add(e);
        foreach (VisualEffect e in volumeEffects)
            effects.Add(e);

        ApplySavedSettings();

        foreach (VisualEffect e in effects)
            e.SetToDefault();
    }

    public void ApplySavedSettings()
    {
        VisualEffectSettings v = GameData.vfxSettings;
        for(int i = 0; i < v.toggle.Count; i++)
        {
            foreach (VisualEffectBoolean e in toggleEffects)
                if (e.name == v.toggle[i])
                    e.defaultValue = v.toggleVal[i];
        }
        for (int i = 0; i < v.slider.Count; i++)
        {
            foreach (VisualEffectSlider e in sliderEffects)
                if (e.name == v.slider[i])
                    e.defaultValue = v.sliderVal[i];
        }
        for (int i = 0; i < v.volume.Count; i++)
        {
            foreach (GameObjectToggle e in volumeEffects)
                if (e.name == v.volume[i])
                    e.defaultValue = v.volumeVal[i];
        }

        float shake = GameData.vfxSettings.camShake;
        Camera.main.GetComponent<CameraFollow>().shakeMultiplier = shake;
        camShakeSlider.value = shake;

        shake = GameData.vfxSettings.shakeFreq;
        Camera.main.GetComponent<CameraFollow>().shakeFrequency = shake;
        shakeFreqSlider.value = shake;
    }

    private void SaveSettings()
    {
        VisualEffectSettings v = GameData.vfxSettings;
        for (int i = 0; i < v.toggle.Count; i++)
        {
            foreach (VisualEffectBoolean e in toggleEffects)
                if (e.name == v.toggle[i])
                    v.toggleVal[i] = e.toggle == 1;
        }
        for (int i = 0; i < v.slider.Count; i++)
        {
            foreach (VisualEffectSlider e in sliderEffects)
                if (e.name == v.slider[i])
                    v.sliderVal[i] = Mathf.Pow(e.slider.value, e.exponentialModifier);
        }
        for (int i = 0; i < v.volume.Count; i++)
        {
            foreach (GameObjectToggle e in volumeEffects)
                if (e.name == v.volume[i])
                    v.volumeVal[i] = e.toggle;
        }
        GameController.SaveGameData();
    }

    public void ToggleEffect(string s)
    {
        foreach (VisualEffect e in effects)
            if (e.name == s)
            {
                e.ToggleEffect();
                SaveSettings();
                return;
            }
    }

    public void SetCamShake()
    {
        Camera.main.GetComponent<CameraFollow>().shakeMultiplier = camShakeSlider.value;
        GameData.vfxSettings.camShake = camShakeSlider.value;
        GameController.SaveGameData();
    }

    public void SetCamShakeFrequency()
    {
        Camera.main.GetComponent<CameraFollow>().shakeFrequency = shakeFreqSlider.value;
        GameData.vfxSettings.shakeFreq = shakeFreqSlider.value;
        GameController.SaveGameData();
    }

    [System.Serializable]
    public class VisualEffectBoolean : VisualEffect
    {
        public bool defaultValue = false;
        public Image checkbox;
        public int toggle = 0;

        public override void SetToDefault()
        {
            toggle = defaultValue ? 1 : 0;
            Shader.SetGlobalInt(variable, toggle);
            checkbox.sprite = toggle == 1 ? tBox : fBox;
        }

        public override void ToggleEffect()
        {
            toggle = toggle == 1 ? 0 : 1;
            Shader.SetGlobalInt(variable, toggle);
            checkbox.sprite = toggle == 1 ? tBox : fBox;
        }
    }

    [System.Serializable]
    public class VisualEffectSlider : VisualEffect
    {
        public float defaultValue;
        public Slider slider;
        public float exponentialModifier = 1;

        public override void SetToDefault()
        {
            slider.value = Mathf.Pow(defaultValue, 1 / exponentialModifier);
            ToggleEffect();
        }

        public override void ToggleEffect()
        {
            Shader.SetGlobalFloat(variable, Mathf.Pow(slider.value, exponentialModifier));
        }
    }

    [System.Serializable]
    public class GameObjectToggle : VisualEffect
    {
        public GameObject obj;
        public Image checkbox;
        public bool defaultValue;
        public bool toggle;

        public override void SetToDefault()
        {
            toggle = defaultValue;
            obj.SetActive(toggle);
            checkbox.sprite = toggle ? tBox : fBox;
        }

        public override void ToggleEffect()
        {
            toggle = !toggle;
            obj.SetActive(toggle);
            checkbox.sprite = toggle ? tBox : fBox;
        }
    }

    public class VisualEffect
    {
        public string name, variable;
        public static Sprite tBox, fBox;

        public virtual void SetToDefault() { }
        public virtual void ToggleEffect() { }
    }
}
[System.Serializable]
public struct VisualEffectSettings
{
    public List<string> toggle, slider, volume;
    public List<bool> toggleVal, volumeVal;
    public List<float> sliderVal;
    public float camShake, shakeFreq;

    public VisualEffectSettings(bool b = true)
    {
        toggle = new List<string>();
        slider = new List<string>();
        volume = new List<string>();
        toggleVal = new List<bool>();
        volumeVal = new List<bool>();
        sliderVal = new List<float>();

        toggle.Add("dmono");
        toggleVal.Add(false);
        toggle.Add("vignette");
        toggleVal.Add(true);

        slider.Add("cspace");
        sliderVal.Add(16);
        slider.Add("camshake");
        sliderVal.Add(1);

        volume.Add("CRT");
        volumeVal.Add(false);
        camShake = 2;
        shakeFreq = 2;
    }
}
