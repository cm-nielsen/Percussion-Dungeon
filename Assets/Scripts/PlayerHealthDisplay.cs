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
        barSegments.Last().canDrain = true;
        for(int i = barSegments.Count - 1; i > 0; i--)
        {
            barSegments[i].setNextSegment(barSegments[i - 1]);
        }

        AdjustSegmentCount(6);
        AdjustSegmentCount(8);
        //AdjustSegmentCount(1);
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
        for (int i = barSegments.Count - 1; i > 0; i--)
        {
            barSegments[i].setNextSegment(barSegments[i - 1]);
        }
    }
}
