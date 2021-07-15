using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboDisplay : MonoBehaviour, IReceiveDamage
{
    public float comboTime = 1;

    private CameraOverrideArea camArea;
    private VerticalTextLayout hits, total;

    private float comboTotal = 0;
    private float comboTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        camArea = GetComponent<CameraOverrideArea>();

        VerticalTextLayout[] textDisplays = GetComponentsInChildren<VerticalTextLayout>();
        hits = textDisplays[0];
        total = textDisplays[1];
        camArea.CallOnReset(hits.Clear);
        camArea.CallOnReset(() => total.gameObject.SetActive(false));
        camArea.CallOnTrigger(() => total.gameObject.SetActive(true));
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newScale = new Vector2(1, 1);
        if (transform.parent.lossyScale.x < 1)
            newScale = new Vector2(-1, 1);
        transform.localScale = newScale;

        if (comboTimer > comboTime)
            hits.Clear();
        comboTimer += Time.deltaTime;
    }

    public void Receive(float amount)
    {
        camArea.Trigger();
        hits.Append(string.Format("{0:0.00}", amount));
        comboTotal += amount;
        if (comboTimer < comboTime)
            total.ModifyNewest(string.Format("{0:0.00}", comboTotal));
        else
        {
            comboTotal = amount;
            total.Append(string.Format("{0:0.00}", comboTotal));
        }
        comboTimer = 0;
    } 
}

public interface IReceiveDamage
{
    void Receive(float amount);
}
