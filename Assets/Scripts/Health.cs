using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int max;
    public float amount;

    public HealthDisplay display;
    // Start is called before the first frame update
    void Start()
    {
        amount = max;

        if (!display)
            display = GetComponent<HealthDisplay>();
        if (!display)
            display = GetComponentInChildren<HealthDisplay>();
    }

    public void Reduce(float dam)
    {
        amount -= dam;
        if (amount <= 0)
        {
            amount = 0;
            OnDeath();
        }

        if (display)
            display.UpdateDisplay(amount / max);
    }

    public void Heal(float am)
    {
        amount += am;
        if (amount > max)
            amount = max;

        if (display)
            display.UpdateDisplay(amount / max);
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}
