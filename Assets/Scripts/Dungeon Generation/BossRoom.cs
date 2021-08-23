using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class BossRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tilemap map = GetComponentInChildren<Tilemap>();
        map.CompressBounds();

        Tilemap parentMap = GetComponentInParent<Tilemap>();
        WallPeel peel = GetComponent<WallPeel>();
        peel.map = parentMap;
        peel.Initialize();

        BoundsInt bounds = map.cellBounds;
        TileBase[] tiles = map.GetTilesBlock(bounds);

        Vector3 offset = transform.position;
        offset.x /= map.cellSize.x;
        offset.y /= map.cellSize.y;
        bounds.position += Vector3Int.FloorToInt(offset);

        parentMap.SetTilesBlock(bounds, tiles);
        Destroy(map.gameObject);
        Destroy(GetComponent<Grid>());
    }
}
