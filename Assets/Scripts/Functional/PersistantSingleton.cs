using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantSingleton : MonoBehaviour
{
    private static Dictionary<string, GameObject> set = new Dictionary<string, GameObject>();

    public string id;
    void Start()
    {
        if (!set.ContainsKey(id))
        {
            set.Add(id, gameObject);
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }
}
