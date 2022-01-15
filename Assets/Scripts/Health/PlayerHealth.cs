using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public int upgradeAmount;
    // Start is called before the first frame update
    void Start()
    {
        amount = max;
        GameController gcon = GameObject.FindObjectOfType<GameController>();
        if (!gcon)
            return;

        upgradeAmount = gcon.healthIncrement;
        FindDisplay();
        max = (GameData.healthUpgrades + gcon.minHealthUpgrades - 1) * upgradeAmount;

        UpgradeMax();
    }

    public override void OnDeath()
    {
        Destroy(GetComponent<PlayerController>());
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
}
