using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    private static GameController instance = null;

    public int healthIncrement = 5, minHealthUpgrades;
    public GameObject currentWeaponPrefab;
    public WeaponSet weaponSet;
    public float profIncrement, LevelIncrement;

    [Space(10)]
    public bool debugPrint;

    private void OnEnable()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadFromFile();
            SceneManager.sceneLoaded += ApplyParameters;
            return;
        }
        Destroy(gameObject);
    }

    private void LoadFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        //WipeSave();
        if (Application.platform != RuntimePlatform.WebGLPlayer) {
            if (File.Exists(Application.persistentDataPath + "/save.drum"))
            {
                try
                {
                    FileStream fStream = File.Open(Application.persistentDataPath + "/save.drum",
                        FileMode.Open);
                    GameDataInstance save = (GameDataInstance)bf.Deserialize(fStream);
                    GameData.Load(save);
                    fStream.Close();
                }
                catch (System.Runtime.Serialization.SerializationException e)
                {
                    if (debugPrint)
                    {
                        print("failed to load from file");
                        print(e);
                    }
                    WipeSave();
                }
            }
            else
                WipeSave();
        }
        else
        {
            if (PlayerPrefs.HasKey("Save"))
            {
                try
                {
                    print("Loading from player prefs");
                    byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(
                        PlayerPrefs.GetString("Save"));
                    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        GameDataInstance save = (GameDataInstance)
                            bf.Deserialize(stream);
                        GameData.Load(save);
                    }
                }
                catch (System.Runtime.Serialization.SerializationException e)
                {
                    if (debugPrint)
                    {
                        print("failed to load from cookies");
                        print(e);
                    }
                    WipeSave();
                }
                catch (System.Exception e)
                {
                    if (debugPrint)
                    {
                        print("failed to load from cookies");
                        print(e);
                    }
                    WipeSave();
                }
            }
            else
                WipeSave();
        }
    }

    public void ApplyParameters(Scene s, LoadSceneMode m)
    {
        currentWeaponPrefab = weaponSet.GetWeapon(GameData.current);
    }

    public static void SaveGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            FileStream fStream = File.Create(
                Application.persistentDataPath + "/save.drum");
            GameDataInstance save = new GameDataInstance(false);
            //foreach (ControlKey.ControlUnit u in save.pControls)
            //    Debug.Log(u.identifier);
            bf.Serialize(fStream, save);
            fStream.Close();
            //Debug.Log("GameData saved");
        }
        else
        {
            print("Saving to pPlayerPrefs");
            using (MemoryStream stream = new MemoryStream())
            {
                bf.Serialize(stream, new GameDataInstance(false));
                PlayerPrefs.SetString("Save",
                    System.Text.Encoding.ASCII.
                    GetString(stream.GetBuffer()));
            }
            print("Value Saved in player prefs: " +
                PlayerPrefs.GetString("Save"));
        }

        if (instance.debugPrint)
            print("Saved GameData");
    }

    public static void WipeSave()
    {
        GameData.healthUpgrades = 0;
        GameData.castas = 0;
        GameData.masterVol = GameData.musicVol = GameData.sfxVol = .5f;
        GameData.unlocks = WeaponUnlocks.drumsticks;
        GameData.current = WeaponUnlocks.drumsticks;

        GameData.vfxSettings = new VisualEffectSettings(true);

        GameData.experience = new WeaponExperience();

        GameData.pControls = new List<ControlKey.ControlUnit>();
        foreach (ControlKey.ControlUnit u in GameObject.FindGameObjectWithTag("pControl").
            GetComponent<ControlKey>().inputs)
            GameData.pControls.Add(u);

        GameData.colours = FindObjectOfType
            <ColourSettingsInstance>().ApplyDefault();

        //if (Application.platform != RuntimePlatform.WebGLPlayer)
            SaveGameData();
    }

    public static void WipeProgress()
    {
        GameData.healthUpgrades = 0;
        GameData.castas = 0;
        GameData.unlocks = WeaponUnlocks.drumsticks;
        GameData.current = WeaponUnlocks.drumsticks;

        GameData.experience = new WeaponExperience();

        //if (Application.platform != RuntimePlatform.WebGLPlayer)
            SaveGameData();
    }

    public static void WipeSettings()
    {
        GameData.masterVol = GameData.musicVol = GameData.sfxVol = .5f;
        AudioMenu.SetAll();
        GameData.vfxSettings = new VisualEffectSettings(true);

        PauseMenu menu = FindObjectOfType<PauseMenu>();
        menu.GetComponentInChildren<VisualEffectMenu>(true).ApplySavedSettings();
        menu.GetComponentInChildren<ControlKeyCustomizationMenu>(true).SetToDefaults();
        menu.GetComponentInChildren<ColourPresetMenu>(true).ApplyPreset(0);

        foreach (ControlKey.ControlUnit u in GameObject.FindGameObjectWithTag("pControl").
            GetComponent<ControlKey>().inputs)
            GameData.pControls.Add(new ControlKey.ControlUnit(u));

        //if (Application.platform != RuntimePlatform.WebGLPlayer)
            SaveGameData();
    }

    public void SetCurrentWeap(GameObject weap)
    {
        currentWeaponPrefab = weap;
        GameData.current = weaponSet.GetType(weap);
        GainExp(0);
    }

    public static void GainExp(int amount, bool save = true)
    {
        if (!instance) return;


        float rat = GameData.experience.AddExperience(amount, GameData.current);
        //Debug.Log("level : " + GameData.experience.LevelOf(GameData.current) +
        //    "Ratio :" + rat);
        if (save)
            SaveGameData();

        FindObjectOfType<PlayerController>()?.UpdateDamage();
        ExperienceBarDisplay bar = FindObjectOfType<ExperienceBarDisplay>();
        if (!bar)
            return;
        bar.UpdateDisplay(rat, GameData.experience.LevelOf(GameData.current));
    }

    public static float GetDamageMod()
    {
        if (!instance)
            return 1;
        float f = GameData.experience.LevelOf(GameData.current) * instance.profIncrement;
        return 1 + f + GameData.experience.totalLevel * instance.LevelIncrement;
    }
    /// <summary>
    /// Fetches level info for the currently selected weapon
    /// </summary>
    /// <returns>An array in the format [weapon level, weapon damage bonus, total level, total damage bonus]</returns>
    public float[] GetLevelInfo()
    {
        float[] ar = new float[4];
        ar[0] = GameData.experience.LevelOf(GameData.current);
        ar[1] = ar[0] * profIncrement;
        ar[2] = GameData.experience.totalLevel;
        ar[3] = ar[2] * LevelIncrement;
        return ar;
    }

    public bool UnlockWeapon(GameObject prefab, int n)
    {
        if (n > GameData.castas)
            return false;
        GameData.castas -= n;
        GameData.unlocks |= weaponSet.GetType(prefab);
        SaveGameData();
        print("Game Saved from:" + name);
        return true;
    }

    public bool IsUnlocked(GameObject prefab)
    {
        if (weaponSet.GetType(prefab) == 0)
            return true;
        return (GameData.unlocks & weaponSet.GetType(prefab)) != 0;
    }

    public static GameObject GetCurrentWeaponPrefab()
    {
        if (instance.currentWeaponPrefab)
            return instance.currentWeaponPrefab;

        GameObject g = instance.weaponSet.GetWeapon(WeaponUnlocks.drumsticks);
        instance.currentWeaponPrefab = g;
        return g;
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

    public static List<ControlKey.ControlUnit> pControls;

    public static ColourSettings colours;

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
        pControls = i.pControls;
        colours = i.colours;
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
    public List<ControlKey.ControlUnit> pControls;
    public ColourSettings colours;

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
        pControls = GameData.pControls;
        colours = GameData.colours;
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
                return (n - prev) /(f - prev);
            prev = f;
            i++;
        }
    }
}
