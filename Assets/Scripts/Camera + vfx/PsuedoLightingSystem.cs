using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsuedoLightingSystem : MonoBehaviour
{
    public float playerRadius, enemyRadius, baseLight;
    public int maxPoints = 1000;

    public Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector4> data = new List<Vector4>();
        PlayerController p = FindObjectOfType<PlayerController>();
        Vector3 v = Vector3.zero;
        if (p)
        {
            v = p.transform.position;
            v.z = playerRadius;
            data.Add(v);
        }
        foreach(EnemyBehavior e in FindObjectsOfType<EnemyBehavior>())
        {
            v = e.transform.position;
            v.z = enemyRadius;
            data.Add(v);
            if (data.Count == maxPoints)
                break;
        }

        //if (data.Count > 1) {
        //    string s = "Data: ";
        //    foreach (Vector4 vec in data)
        //        s += vec.ToString() + ", ";
        //    print(s);
        //}

        while (data.Count < maxPoints)
            data.Add(Vector4.zero);

        foreach(Material m in materials)
        {
            m.SetVectorArray("_PositionData", data);
            m.SetInt("_PosCount", data.Count);
            m.SetFloat("_BaseLight", baseLight);
        }
    }
}
