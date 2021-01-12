using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpFromPoint : MonoBehaviour
{
    public float distance, val;

    private Vector2 startPos, endPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        endPos = startPos + distance * Random.insideUnitCircle;
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, endPos, val);

        if (Vector2.Distance(transform.position, endPos) < distance / 10000)
            Destroy(gameObject);
    }

    public void Initiate(Material mat, float n)
    {

        Text t = GetComponent<Text>();
        if (t)
        {
            t.material = mat;
            t.text = n.ToString();
        }
    }
}
