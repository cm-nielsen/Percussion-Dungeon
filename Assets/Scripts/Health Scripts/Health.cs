using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int max, upgradeAmount;
    public float amount;

    public HealthDisplay display;
    // Start is called before the first frame update
    void Start()
    {
        amount = max;

        FindDisplay();
    }

    public void FindDisplay()
    {
        if (GetComponent<PlayerController>())
            display = GameObject.FindGameObjectWithTag("pHealth").GetComponent<SegmentedHealthBarDisplay>();

        if (!display)
            display = GetComponent<HealthDisplay>();
        if (!display)
            display = GetComponentInChildren<HealthDisplay>();
    }
    /// <summary>
    /// reduces health by goven value, returns true if target has been killed
    /// </summary>
    public bool Reduce(float dam)
    {
        bool dead = false;
        amount -= dam;
        if (amount <= 0)
        {
            amount = 0;
            OnDeath();
            dead = true;
        }

        if (display)
            display.UpdateDisplay(amount / max);

        return dead;
    }

    public bool Heal(float am)
    {
        bool change = amount < max;
        amount += am;
        if (amount > max)
            amount = max;

        if (display)
            display.UpdateDisplay(amount / max);

        return change;
    }

    public void UpgradeMax()
    {
        max += upgradeAmount;
        amount = max;

        if (display)
        {
            if (display is SegmentedHealthBarDisplay)
                (display as SegmentedHealthBarDisplay).AdjustSegmentCount(max / upgradeAmount);
            display.UpdateDisplay(amount / max);
        }
    }

    private void OnDeath()
    {
        //Debug.Log(gameObject.name + " has been killed.");
        //Destroy(gameObject);
    }
}
