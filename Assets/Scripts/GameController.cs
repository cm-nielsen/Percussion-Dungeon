using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameObject instance = null;

    public int healthIncrement = 5, minHealthUpgrades;

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
        GameData.castas = 1;
    }

    public void ApplyParameters(Scene s, LoadSceneMode m)
    {
        Health h = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        h.upgradeAmount = healthIncrement;
        h.FindDisplay();
        h.max = (GameData.healthUpgrades + minHealthUpgrades - 1) * healthIncrement;
        h.UpgradeMax();
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

    public static WeaponUnlocks unlocks;
}
