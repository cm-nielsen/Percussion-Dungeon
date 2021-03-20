using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterialToCamera : MonoBehaviour
{
    public List<Material> mats;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        foreach (Material m in mats)
            Graphics.Blit(source, destination, m);
    }
}
