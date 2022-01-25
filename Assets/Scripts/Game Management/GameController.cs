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
                catch (System.Exception e)
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
            if (PlayerPrefs.HasKey("health upgrades"))
            {
                try
                {
                    if (debugPrint)
                        print("loading from Cookies");
                    GameDataInstance save = new GameDataInstance();
                    save.ReadFromPrefs();
                    GameData.Load(save);
                }catch (System.Exception e)
                {
                    if (debugPrint)
                    {
                        print("failed to read from Cookies");
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

            bf.Serialize(fStream, save);
            fStream.Close();
        }
        else
        {
            if (instance.debugPrint)
                print("Saving to pPlayerPrefs");

            PlayerPrefs.DeleteAll();
            new GameDataInstance(false).SaveToPrefs();
            if (instance.debugPrint)
            {
                print("Value Saved in player prefs: " +
                    PlayerPrefs.GetInt("health upgrades", 0) + "\t:::" +
                    PlayerPrefs.GetInt("castas", 0) + "\t:::" +
                    PlayerPrefs.GetFloat("master volume", .5f) + "\t:::" +
                    PlayerPrefs.GetFloat("music volume", .5f) + "\t:::" +
                    PlayerPrefs.GetFloat("sfx volume", .5f) + "\t:::" +
                    PlayerPrefs.GetInt("unlocks", 0) + "\t:::" +
                    PlayerPrefs.GetInt("current", 0) + "\t:::" +
                    PlayerPrefs.GetString("vfx settings") + "\t:::" +
                    PlayerPrefs.GetString("weapon experience") + "\t:::" +
                    PlayerPrefs.GetString("controls") + "\t:::" +
                    PlayerPrefs.GetString("colours"));
            }
        }

        if (instance.debugPrint)
            print("Saved GameData");
    }

    public static void WipeSave()
    {
        GameData.healthUpgrades = 0;
        GameData.castas = 0;
        GameData.masterVol = GameData.musicVol = .5f;
        GameData.sfxVol = 1;
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
        PauseMenu.vfxMenu.ApplySavedSettings();

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
        GameData.masterVol = GameData.musicVol = .5f;
        GameData.sfxVol = 1;
        AudioMenu.SetAll();
        GameData.vfxSettings = new VisualEffectSettings(true);

        PauseMenu menu = FindObjectOfType<PauseMenu>();
        menu.GetComponentInChildren<VisualEffectMenu>(true).ApplySavedSettings();
        menu.GetComponentInChildren<ControlKeyCustomizationMenu>(true).SetToDefaults();
        menu.GetComponentInChildren<ColourPresetMenu>(true).ApplyPreset(0);

        foreach (ControlKey.ControlUnit u in GameObject.FindGameObjectWithTag("pControl").
            GetComponent<ControlKey>().inputs)
            GameData.pControls.Add(new ControlKey.ControlUnit(u));

        GameData.colours = FindObjectOfType
            <ColourSettingsInstance>().ApplyDefault();
        PauseMenu.vfxMenu.ApplySavedSettings();

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
        //print("Game Saved from:" + name);
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
