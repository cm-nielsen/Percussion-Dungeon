using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerHealthDisplay : HealthDisplay
{
    public Transform barEnd;
    public GameObject midBarPrefab;

    public float pointsPerSegment;

    public List<HealthBarDisplay> barSegments;
    // Start is called before the first frame update
    void Start()
    {
        barSegments = GetComponentsInChildren<HealthBarDisplay>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateDisplay(float ratio)
    {
        ratio *= barSegments.Count;

        foreach(HealthBarDisplay bd in barSegments)
        {
            bd.UpdateDisplay(Mathf.Clamp01(ratio));
            ratio -= 1;
        }
    }

    public void AdjustMax(float newMax)
    {

    }
}
