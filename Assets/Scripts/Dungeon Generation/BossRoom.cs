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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public TilePositionSet FetchTilePositionSet(IEnumerable<TileBase> tileset, Vector3Int offset)
    {
        Tilemap map = GetComponentInChildren<Tilemap>();
        BoundsInt bounds = map.cellBounds;

        TilePositionSet set = new TilePositionSet();
        Vector3Int v = -Vector3Int.FloorToInt(bounds.center - bounds.min);
        TileBase[] tiles = map.GetTilesBlock(bounds);

        v += offset;
        Vector3Int max = bounds.max;
        Vector3Int[] positions = new Vector3Int[tiles.Length];
        int i = 0, x = v.x;
        while (v.y < max.y)
        {
            v.x = x;
            while (v.x < max.x)
            {
                positions[i++] = v;
                v.x++;
            }
            v.y++;
        }

        List<int> valid = new List<int>();
        for (i = 0; i < tiles.Length; i++)
            if (tileset.Contains(tiles[i]))
                valid.Add(i);
        foreach (int ind in valid)
        {
            set.tiles.Add(tiles[ind]);
            set.positions.Add(positions[ind]);
        }

        Destroy(map);
        return set;
    }
}
