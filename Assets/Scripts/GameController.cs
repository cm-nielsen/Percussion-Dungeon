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
}
