using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    private static GameObject instance = null;

    public int healthIncrement = 5, minHealthUpgrades;
    public GameObject currentWeaponPrefab;
    public WeaponSet weaponSet;
    public List<int> levels;

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
        GameData.experience = new WeaponExperience();
        GameData.experience.levels = levels;
        if (File.Exists(Application.persistentDataPath + "/save.drum"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fStream = File.Open(Application.persistentDataPath + "/save.drum",
                FileMode.Open);
            GameData.Load((GameDataInstance)bf.Deserialize(fStream));
            fStream.Close();
            //Debug.Log("GameData Loaded");
        }
    }

    public void ApplyParameters(Scene s, LoadSceneMode m)
    {
        currentWeaponPrefab = weaponSet.GetWeapon(GameData.current);
    }

    public static void SaveGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fStream = File.Create(Application.persistentDataPath + "/save.drum");
        bf.Serialize(fStream, new GameDataInstance(false));
        fStream.Close();
        //Debug.Log("GameData saved");
    }

    public static void WipeGameData()
    {
        GameData.healthUpgrades = 0;
        GameData.castas = 0;
        GameData.unlocks = WeaponUnlocks.drumsticks;
        GameData.current = WeaponUnlocks.rainstick;

        GameData.vfxSettings = new VisualEffectSettings(true);

        GameData.experience = new WeaponExperience();
        SaveGameData();
    }

    public void SetCurrentWeap(GameObject weap)
    {
        currentWeaponPrefab = weap;
        GameData.current = weaponSet.GetType(weap);
    }

    public static void GainExp(int amount)
    {
        float rat = GameData.experience.AddExperience(amount, GameData.current);
        //Debug.Log("level : " + GameData.experience.LevelOf(GameData.current) +
        //    "Ratio :" + rat);
        SaveGameData();
        ExperienceBarDisplay bar = FindObjectOfType<ExperienceBarDisplay>();
        if (!bar)
            return;
        bar.UpdateDisplay(rat);
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

    public static float masterVol, musicVol, sfxVol;

    public static WeaponUnlocks unlocks, current;

    public static VisualEffectSettings vfxSettings;

    public static WeaponExperience experience;

    public static void Load(GameDataInstance i)
    {
        healthUpgrades = i.healthUpgrades;
        castas = i.castas;
        masterVol = i.masterVol;
        musicVol = i.musicVol;
        sfxVol = i.sfxVol;
        unlocks = i.unlocks;
        current = i.current;
        vfxSettings = i.vfxSettings;
        experience = i.experience;
    }
}

[System.Serializable]
public struct GameDataInstance
{
    public int healthUpgrades, castas;
    public float masterVol, musicVol, sfxVol;
    public WeaponUnlocks unlocks, current;
    public VisualEffectSettings vfxSettings;
    public WeaponExperience experience;

    public GameDataInstance(bool b = false)
    {
        healthUpgrades = GameData.healthUpgrades;
        castas = GameData.castas;
        masterVol = GameData.masterVol;
        musicVol = GameData.musicVol;
        sfxVol = GameData.sfxVol;
        unlocks = GameData.unlocks;
        current = GameData.current;
        vfxSettings = GameData.vfxSettings;
        experience = GameData.experience;
    }
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

[System.Serializable]
public class WeaponExperience
{
    public List<int> levels;
    public int drumsticks, hang, rainstick, bongos,
            triangle, cowbell, cymbals, marracas;
    public int totalLevel { get {
            return (LevelOf(drumsticks) + LevelOf(hang) + LevelOf(rainstick) +
                    LevelOf(bongos) + LevelOf(triangle) + LevelOf(cowbell) +
                    LevelOf(cymbals) + LevelOf(marracas)) / 4;
        } }

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
        for(int i = 0; i < levels.Count; i++)
        {
            if(levels[i] > n)
                return i;
        }
        return levels.Count;
        //int res = 0;
        //foreach (int i in levels)
        //    if (n > i)
        //        res++;
        //    else
        //        return res;
        //return res;
    }

    private float RemainingRatio(int n)
    {
        int prev = 0;
        foreach (int i in levels)
            if (n < i)
                return (n - prev) / (float)(i - prev);
            else
                prev = i;
        return 1;
    }
}
