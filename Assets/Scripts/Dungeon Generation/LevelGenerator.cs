using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap map;
    public TileBase tile;

    public int width, height, snakeCount, snakeHopLength;

    // Start is called before the first frame update
    void Start()
    {
        //map.ClearAllTiles();
        //map.BoxFill(Vector3Int.zero, tile, -5, -5, 5, 2);
        //Vector3Int[] ar =
        //{
        //    new Vector3Int(0, 0, 0),
        //    new Vector3Int(1, 0, 0),
        //    new Vector3Int(2, 0, 0),
        //    new Vector3Int(3, 0, 0),
        //    new Vector3Int(4, 0, 0),
        //    new Vector3Int(5, 0, 0),
        //    new Vector3Int(6, 0, 0)
        //};
        //foreach(Vector3Int v in ar)
        //{
        //    map.SetTile(v, tile);
        //}
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        TileBase[,] grid = new TileBase[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                grid[i, j] = tile;

        foreach (Pair p in GenerateSnake())
        {
            grid[p.x, p.y] = null;
            Debug.Log(p.x + ", " + p.y);
        }

        map.ClearAllTiles();
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                map.SetTile(new Vector3Int(i, -j, 0), grid[i, j]);
    }

    private List<Pair> GenerateSnake()
    {
        List<Pair> ls = new List<Pair>();

        ls.Add(new Pair(width / 2, height / 2));
        for(int i = 0; i< snakeCount; i++)
        {
            ls.Add(ls.Last() + Pair.Random(snakeHopLength));
        }
        return ls;
    }

    private class Pair
    {
        public int x, y;
        public Pair(int a =  0, int b = 0) { x = a; y = b; }
        public static Pair Random(int len)
        {
            len *= 2;
            int a = (int)((UnityEngine.Random.value - 0.5f) * len);
            int b = (int)((UnityEngine.Random.value - 0.5f) * len / 5);
            if (UnityEngine.Random.value > 0.5f)
                return new Pair(a, b);
            else
                return new Pair(b, a);
        }
        public static Pair operator +(Pair a, Pair b) 
            => new Pair(a.x + b.x, a.y + b.y);
        public static Pair operator *(Pair a, float f) 
            => new Pair((int)(a.x * f), (int)(a.y * f));
    }
}
