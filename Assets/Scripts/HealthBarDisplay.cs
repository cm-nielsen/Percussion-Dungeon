using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthBarDisplay : HealthDisplay
{
    private SpriteRenderer fill;

    private float maxWidth;
    // Start is called before the first frame update
    void Start()
    {
        fill = GetComponent<SpriteRenderer>();
        fill.drawMode = SpriteDrawMode.Tiled;
        maxWidth = fill.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateDisplay(float ratio)
    {
        fill.size = new Vector2(ratio * maxWidth, fill.size.y);
    }
}
