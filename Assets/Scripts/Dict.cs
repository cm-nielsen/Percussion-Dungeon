using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameObjectDict
{
    public List<DictItem> list;

    public GameObject this[string s]
    {
        get
        { foreach (DictItem unit in list)
                if (unit.identifier.ToLower().Equals(s.ToLower()))
                    return unit.data;
            return null; }
    }

    [Serializable]
    public struct DictItem
    { public string identifier;
        public GameObject data; }
}

[Serializable]
public class ParticleSystemDict
{
    public List<DictItem> list;

    public ParticleSystem this[string s]
    {
        get
        {
            foreach (DictItem unit in list)
                if (unit.identifier.ToLower().Equals(s.ToLower()))
                    return unit.data;
            return null;
        }
    }

    [Serializable]
    public struct DictItem
    {
        public string identifier;
        public ParticleSystem data;
    }
}
