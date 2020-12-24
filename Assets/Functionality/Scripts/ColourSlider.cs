using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourSlider : MonoBehaviour
{
    public string propertyName;
    public Slider r, g, b;
    private Material mat;
    private RectTransform tr;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (mat == null)
        //    return;

        //Color c = new Color(r.value, g.value, b.value);
        //mat.SetColor(propertyName, c);
    }

    public void setMaterial(Material m)
    {
        mat = m;
        Color c = m.GetColor(propertyName);

        r.value = c.r;
        g.value = c.g;
        b.value = c.b;
    }

    public void UpdateMaterial()
    {
        if (mat == null)
            return;
        mat.SetColor(propertyName, new Color(r.value, g.value, b.value));

        GameObject.FindObjectOfType<UIPointer>().GetComponent<UIPointer>().UpdatePosition(
            transform.position + Vector3.left * 0.5f, Quaternion.identity);
        Debug.Log(tr.localPosition);
    }
}
