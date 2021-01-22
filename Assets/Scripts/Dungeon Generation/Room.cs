using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Flags]
public enum Doors
{
    none = 0,
    up = 1,
    down = 2,
    left = 4,
    right = 8
}

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    public Doors doors = Doors.none;

    public Tilemap map;

    public Vector2Int size;

    // Start is called before the first frame update
    void Start()
    {
        UpdateAttributes();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAttributes();
    }

    public void UpdateAttributes()
    {
        map = GetComponent<Tilemap>();
        map.CompressBounds();
        size = (Vector2Int)map.cellBounds.size;

        doors = Doors.none;
        Vector3Int doorCheck = new Vector3Int(0, -size.y / 2, 0);
        if (!map.GetTile(doorCheck))
            doors |= Doors.down;
        doorCheck.y = -doorCheck.y - 1;
        if (!map.GetTile(doorCheck))
            doors |= Doors.up;
        doorCheck.y = 0;
        doorCheck.x = -size.x / 2;
        if (!map.GetTile(doorCheck))
            doors |= Doors.left;
        doorCheck.x = -doorCheck.x - 1;
        if (!map.GetTile(doorCheck))
            doors |= Doors.right;
    }

    public void PrintToLog()
    {
        Debug.Log(gameObject.name);
    }
}
