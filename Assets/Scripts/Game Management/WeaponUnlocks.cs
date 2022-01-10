using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponExperience
{
    public int drumsticks, hang, rainstick, bongos,
            triangle, cowbell, cymbals, marracas;

    public WeaponExperience()
    {
        drumsticks = hang = rainstick = bongos =
            triangle = cowbell = cymbals = marracas = 0;
    }

    public WeaponExperience(string s)
    {
        string[] vals = s.Split(',');
        int i = 0;

        drumsticks = int.Parse(vals[i++]);
        hang = int.Parse(vals[i++]);
        rainstick = int.Parse(vals[i++]);
        bongos = int.Parse(vals[i++]);
        triangle = int.Parse(vals[i++]);
        cowbell = int.Parse(vals[i++]);
        cymbals = int.Parse(vals[i++]);
        marracas = int.Parse(vals[i++]);
    }

    public override string ToString()
    {
        return "" + drumsticks + ',' + hang + ',' +
            rainstick + ',' + bongos + ',' +
            triangle + ',' + cowbell + ',' +
            cymbals + ',' + marracas;
    }

    public int totalLevel
    {
        get
        {
            return (LevelOf(drumsticks) + LevelOf(hang) + LevelOf(rainstick) +
                    LevelOf(bongos) + LevelOf(triangle) + LevelOf(cowbell) +
                    LevelOf(cymbals) + LevelOf(marracas)) / 4;
        }
    }

    public float AddExperience(int amount, WeaponUnlocks u)
    {
        switch (u)
        {
            case WeaponUnlocks.drumsticks:
                drumsticks += amount;
                return RemainingRatio(drumsticks);
            case WeaponUnlocks.hang:
                hang += amount;
                return RemainingRatio(hang);
            case WeaponUnlocks.rainstick:
                rainstick += amount;
                return RemainingRatio(rainstick);
            case WeaponUnlocks.bongos:
                bongos += amount;
                return RemainingRatio(bongos);
            case WeaponUnlocks.triangle:
                triangle += amount;
                return RemainingRatio(triangle);
            case WeaponUnlocks.cowbell:
                cowbell += amount;
                return RemainingRatio(cowbell);
            case WeaponUnlocks.cymbals:
                cymbals += amount;
                return RemainingRatio(cymbals);
            case WeaponUnlocks.marracas:
                marracas += amount;
                return RemainingRatio(marracas);
        }
        return 0;
    }

    public int LevelOf(WeaponUnlocks u)
    {
        switch (u)
        {
            case WeaponUnlocks.drumsticks:
                return LevelOf(drumsticks);
            case WeaponUnlocks.hang:
                return LevelOf(hang);
            case WeaponUnlocks.rainstick:
                return LevelOf(rainstick);
            case WeaponUnlocks.bongos:
                return LevelOf(bongos);
            case WeaponUnlocks.triangle:
                return LevelOf(triangle);
            case WeaponUnlocks.cowbell:
                return LevelOf(cowbell);
            case WeaponUnlocks.cymbals:
                return LevelOf(cymbals);
            case WeaponUnlocks.marracas:
                return LevelOf(marracas);
        }
        return drumsticks;
    }

    private int LevelOf(int n)
    {
        int i = 0;
        while (true)
        {
            if (Mathf.Pow(i, 2) * 10 >= n)
                return i;
            i++;
        }
    }

    private float RemainingRatio(int n)
    {
        int i = 1;
        float prev = 0;

        while (true)
        {
            float f = Mathf.Pow(i, 2) * 10;
            if (f >= n)
                return (n - prev) / (f - prev);
            prev = f;
            i++;
        }
    }
}
