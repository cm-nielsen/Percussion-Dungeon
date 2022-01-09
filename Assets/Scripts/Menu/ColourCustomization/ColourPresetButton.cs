using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColourPresetButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private Action<int> select, press;
    private MoveObjectToPoint pointerMove;
    private int presetIndex;

    public void Initialize(int i, Action<int> s, Action<int> p, string name)
    {
        presetIndex = i;
        select = s;
        press = p;


        Text t = GetComponent<Text>();
        t.text = name;
        RectTransform tr = GetComponent<RectTransform>();
        tr.sizeDelta = new Vector2(name.Length * .75f, 1.5f);
        tr.localPosition = new Vector2(10.5f - (name.Length * t.fontSize * 0.1875f), 2 - (1.5f * i));

        pointerMove = GetComponent<MoveObjectToPoint>();
        pointerMove.pos.x = -.25f - (tr.sizeDelta.x * tr.lossyScale.x / 2);

        GetComponent<Button>().onClick.AddListener(OnPress);
    }

    public void OnPointerEnter(PointerEventData d)
    {
        select(presetIndex);
        pointerMove.ApplyParametersToObject();
    }

    public void OnSelect(BaseEventData d)
    {
        select(presetIndex);
        pointerMove.ApplyParametersToObject();
    }

    private void OnPress()
    {
        press(presetIndex);
    }
}
