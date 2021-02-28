using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameObject instance = null;

    public int healthIncrement = 5, minHealthUpgrades;
    public GameObject currentWeaponPrefab;
    public WeaponSet weaponSet;

    private void OnEnable()
    {
        if (!instance)
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
            LoadFromFile();
            SceneManager.sceneLoaded += ApplyParameters;
            return;
        }
        Destroy(gameObject);
    }

    private void LoadFromFile()
    {
        GameData.healthUpgrades = 0;
        GameData.castas = 0;
        GameData.unlocks = WeaponUnlocks.drumsticks;
        GameData.current = WeaponUnlocks.rainstick;
    }

    public void ApplyParameters(Scene s, LoadSceneMode m)
    {
        //Health h = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        //h.upgradeAmount = healthIncrement;
        //h.FindDisplay();
        //h.max = (GameData.healthUpgrades + minHealthUpgrades - 1) * healthIncrement;
        //h.UpgradeMax();

        currentWeaponPrefab = weaponSet.GetWeapon(GameData.current);
    }

    public static void SaveGameData()
    {

    }

    public void SetCurrentWeap(GameObject weap)
    {
        currentWeaponPrefab = weap;
        GameData.current = weaponSet.GetType(weap);
    }
}

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

public struct GameData
{
    public static int healthUpgrades,
        castas;

    public static WeaponUnlocks unlocks, current;
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
        if (g.name == drumsticks.name)
            return WeaponUnlocks.drumsticks;
        else if (g.name == hang.name)
            return WeaponUnlocks.hang;
        else if (g.name == rainstick.name)
            return WeaponUnlocks.rainstick;
        else if (g.name == bongos.name)
            return WeaponUnlocks.bongos;
        else if (g.name == triangle.name)
            return WeaponUnlocks.triangle;
        else if (g.name == cowbell.name)
            return WeaponUnlocks.cowbell;
        else if (g.name == cymbals.name)
            return WeaponUnlocks.cymbals;
        else if (g.name == marracas.name)
            return WeaponUnlocks.marracas;

        Debug.Log("weapn prefab not found in set");
        return WeaponUnlocks.drumsticks;
    }
}
