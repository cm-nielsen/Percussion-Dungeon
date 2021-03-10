using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffectMenu : MonoBehaviour, RequiresInitialSetup
{
    public List<VisualEffectBoolean> toggleEffects;
    public List<VisualEffectSlider> sliderEffects;
    public List<GameObjectToggle> volumeEffects;
    public Sprite tBox, fBox;
    public Shader effectShader;

    private List<VisualEffect> effects = new List<VisualEffect>();

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

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

        foreach (VisualEffect e in effects)
            e.SetToDefault();
    }

    public void ToggleEffect(string s)
    {
        foreach (VisualEffect e in effects)
            if (e.name == s)
                e.ToggleEffect();
    }

    [System.Serializable]
    public class VisualEffectBoolean : VisualEffect
    {
        public bool defaultValue = false;
        public Image checkbox;
        private int toggle = 0;

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
        private bool toggle;

        public override void SetToDefault()
        {
            toggle = defaultValue;
            obj.SetActive(toggle);
            checkbox.sprite = toggle ? tBox : fBox;
        }

        public override void ToggleEffect()
        {
            Debug.Log("YEET");
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
