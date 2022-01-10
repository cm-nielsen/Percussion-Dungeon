using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum WeaponUnlocks
{
    drumsticks = 0,
    hang = 1,
    rainstick = 2,
    bongos = 4,
    triangle = 8,
    cowbell = 16,
    cymbals = 32,
    marracas = 64
}


[System.Serializable]
public class WeaponSet
{
    public GameObject drumsticks, hang, rainstick, bongos,
            triangle, cowbell, cymbals, marracas;

    public GameObject GetWeapon(WeaponUnlocks u)
    {
        switch (u)
        {
            case WeaponUnlocks.drumsticks:
                return drumsticks;
            case WeaponUnlocks.hang:
                return hang;
            case WeaponUnlocks.rainstick:
                return rainstick;
            case WeaponUnlocks.bongos:
                return bongos;
            case WeaponUnlocks.triangle:
                return triangle;
            case WeaponUnlocks.cowbell:
                return cowbell;
            case WeaponUnlocks.cymbals:
                return cymbals;
            case WeaponUnlocks.marracas:
                return marracas;
        }
        return drumsticks;
    }

    public WeaponUnlocks GetType(GameObject g)
    {
        if (drumsticks && g.name == drumsticks.name)
            return WeaponUnlocks.drumsticks;
        else if (hang && g.name == hang.name)
            return WeaponUnlocks.hang;
        else if (rainstick && g.name == rainstick.name)
            return WeaponUnlocks.rainstick;
        else if (bongos && g.name == bongos.name)
            return WeaponUnlocks.bongos;
        else if (triangle && g.name == triangle.name)
            return WeaponUnlocks.triangle;
        else if (cowbell && g.name == cowbell.name)
            return WeaponUnlocks.cowbell;
        else if (cymbals && g.name == cymbals.name)
            return WeaponUnlocks.cymbals;
        else if (marracas && g.name == marracas.name)
            return WeaponUnlocks.marracas;

        Debug.Log("weapon prefab not found in set");
        return WeaponUnlocks.drumsticks;
    }
}
