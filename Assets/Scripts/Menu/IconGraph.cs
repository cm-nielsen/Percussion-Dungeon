using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconGraph : MonoBehaviour
{
    public GameObject icon;

    public Vector2 origin, offset;

    private List<GameObject> instances = new List<GameObject>();
    private Vector2 pos;
    private int i = 0, j = 0;

    public void Graph(int[] ar)
    {
        Clear();
        StopAllCoroutines();
        pos = origin + (Vector2)transform.position;
        i = j = 0;
        StartCoroutine(GraphCycle(ar));
    }

    public void Clear()
    {
        while(instances.Count > 0)
        {
            Destroy(instances[0]);
            instances.RemoveAt(0);
        }
    }

    private IEnumerator GraphCycle(int[] ar)
    {
        if (i >= ar.Length || j >= ar[i])
            yield break;
        yield return new WaitForEndOfFrame();

        instances.Add(Instantiate(icon, pos, Quaternion.identity, transform));
        pos.x += offset.x;
        j++;

        if (j >= ar[i])
        {
            pos.y -= offset.y;
            pos.x = origin.x + transform.position.x;
            j = 0;
            i++;
        }

        StartCoroutine(GraphCycle(ar));
    }
}
