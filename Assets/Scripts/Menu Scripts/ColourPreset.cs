using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Colour Preset", menuName = "Scriptable Objects/Create Colour Preset", order = 1)]
public class ColourPreset : ScriptableObject, IEnumerable<ColourPair>
{
    public ColourPair player, enemy, platform, background, health, objects, corpse;

    public IEnumerator<ColourPair> GetEnumerator()
    {
        yield return background;
        yield return enemy;
        yield return player;
        yield return objects;
        yield return platform;
        yield return health;
        yield return corpse;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[System.Serializable]
public class ColourPair
{
    public Color main = new Color(1, 1, 1, 1), mono = new Color(0, 0, 0, 1);
}
