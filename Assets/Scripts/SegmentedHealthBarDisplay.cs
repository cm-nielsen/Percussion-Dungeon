using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SegmentedHealthBarDisplay : HealthDisplay
{
    public Transform barEnd;
    public GameObject midBarPrefab;

    public float pointsPerSegment;

    public List<HealthBarDisplay> barSegments;

    public int partialIndex;
    // Start is called before the first frame update
    void Start()
    {
        barSegments = GetComponentsInChildren<HealthBarDisplay>().ToList();

        AssignConnections();

        //AdjustSegmentCount(6);
        //AdjustSegmentCount(8);
        AdjustSegmentCount(1);
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

    public void AdjustSegmentCount(int num)
    {
        if (num <= 2)
        {
            barEnd.localPosition = Vector2.zero;
            RemoveAllMids();
            AssignConnections();
            return;
        }

        barEnd.localPosition = ((num - 2) / 2 * Vector2.right);

        RemoveAllMids();

        for(int i = 2; i < num; i++)
        {
            GameObject g = Instantiate(midBarPrefab, transform);
            g.transform.localPosition = ((i - 2f) / 2 * Vector2.right);
            barSegments.Insert(i - 1, g.GetComponentInChildren<HealthBarDisplay>());
        }

        partialIndex = barSegments.Count - 1;
        AssignConnections();
    }

    private void RemoveAllMids()
    {
        while (barSegments.Count > 2)
        {
            Destroy(barSegments[1].transform.parent.gameObject);
            barSegments.RemoveAt(1);
        }
    }

    private void AssignConnections()
    {
        barSegments.Last().canDrain = true;

        for (int i = 0; i < barSegments.Count; i++)
            barSegments[i].Initialize(this, i);
    }

    public void NotifyDrain(int n)
    {
        partialIndex = n - 1;
        if (partialIndex < 0)
            partialIndex = 0;

        foreach (HealthBarDisplay bd in barSegments)
            bd.canDrain = false;
        barSegments[partialIndex].canDrain = true;
    }

    public void NotifyFill(int n)
    {
        partialIndex = n;
        foreach (HealthBarDisplay bd in barSegments)
            bd.canDrain = false;
        barSegments[partialIndex].canDrain = true;
    }
}
