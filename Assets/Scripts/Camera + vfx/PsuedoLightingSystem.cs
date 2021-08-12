using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsuedoLightingSystem : MonoBehaviour
{
    public MaterialLightingSettings[] members;
    public int maxPoints = 10000;

    // Update is called once per frame
    void Update()
    {
        List<Vector4> data = new List<Vector4>();
        PlayerController p = FindObjectOfType<PlayerController>();
        Vector3 v = Vector3.zero;
        if (p)
        {
            v = p.transform.position;
            v.z = -1;
            data.Add(v);
        }
        foreach(EnemyBehavior e in FindObjectsOfType<EnemyBehavior>())
        {
            v = e.transform.position;
            v.z = 0;
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

            //while (data.Count < maxPoints)
            //    data.Add(Vector4.zero);

            foreach (MaterialLightingSettings m in members)
            m.ApplyData(data, maxPoints);
    }

    [System.Serializable]
    public class MaterialLightingSettings
    {
        public Material mat, colourReference;
        public float playerRadius, enemyRadius, baseLight,
            playerStrength, enemyStrength;

        public float detail = 16;
        public int minX, maxX, minY, maxY;

        public void ApplyData(List<Vector4> a, int maxPoints)
        {
            List<Vector4> data = new List<Vector4>();
            float pr = Mathf.Pow(playerRadius, 2),
                er = Mathf.Pow(enemyRadius, 2);

            foreach(Vector4 v in a)
            {
                if (v.z == -1)
                    data.Add(new Vector4(v.x, v.y, pr, playerStrength));
                else
                    data.Add(new Vector4(v.x, v.y, er, enemyStrength));
            }
            // while (data.Count < maxPoints)
            //data.Add(Vector4.zero);

            mat.SetFloatArray("_PositionData", ComputeLightingArray(data, maxPoints));
            mat.SetInt("_PosArrayWidth", (maxX - minX) * (int)detail);
            mat.SetFloat("_BaseLight", baseLight);

            mat.SetColor("_MainCol", colourReference.GetColor("_MainCol"));
            mat.SetColor("_MonoCol", colourReference.GetColor("_MonoCol"));
        }

        private float[] ComputeLightingArray(List<Vector4> l, int maxPoints)
        {
            float[] ar = new float[maxPoints];
            float d = 1 / detail;
            int X = maxX - minX, Y = maxY - minY;
            if ((X * detail) * (Y * detail) > maxPoints)
            {
                print("Error, proposed lighting limits are too Large to fit in maxPoints");
                return ar;
            }
            int index = 0;
            for (float j = 0; j < Y; j += d)
            {
                for (float i = 0; i < X; i += d)
                {
                    ar[index] = GetLight(l, new Vector2(minX + i, minY + j));
                }
            }
            return ar;
        }

        private float GetLight(List<Vector4> posData, Vector2 pos)
        {
            float l = 0;
            foreach(Vector4 v in posData)
            {
                float x = v.x - pos.x, y = v.y - pos.y;
                l += v.z / (x * x + y * y);
            }
            return l;
        }
    }
}
