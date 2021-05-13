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

[ExecuteAlways]
public class Room : MonoBehaviour
{
    public TileBase platformTile, jarTile;

    public Doors doors = Doors.none;

    public Tilemap map;

    public Vector2Int size;

    public bool rename = false;

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
        if (map.GetTile(doorCheck) != platformTile)
            doors |= Doors.down;
        doorCheck.y = -doorCheck.y - 1;
        if (map.GetTile(doorCheck) != platformTile)
            doors |= Doors.up;
        doorCheck.y = 0;
        doorCheck.x = -size.x / 2;
        if (map.GetTile(doorCheck) != platformTile)
            doors |= Doors.left;
        doorCheck.x = -doorCheck.x - 1;
        if (map.GetTile(doorCheck) != platformTile)
            doors |= Doors.right;

        if (rename)
        {
            string s = "";
            if ((doors & Doors.up) != 0)
                s += "U";
            if ((doors & Doors.down) != 0)
                s += "D";
            if ((doors & Doors.left) != 0)
                s += "L";
            if ((doors & Doors.right) != 0)
                s += "R";
            s += " " + size.x + " x " + size.y;
            transform.parent.gameObject.name = s;
        }

        fillWithJars();
    }

    private void fillWithJars()
    {
        if (!jarTile)
        {
            Debug.Log("Jars were unable to be automatically placed" +
                "Returning to the scene should fix this");
            return;
        }

        Vector3Int v = - ((Vector3Int)size / 2);
        for (int i = 0; i < size.x; i++)
        {
            v.y = -size.y / 2;
            for (int j = 0; j < size.y; j++)
            {
                if (!map.GetTile(v) && ShouldBeJar(v))
                    map.SetTile(v, jarTile);
                if (map.GetTile(v) == jarTile && !ShouldBeJar(v))
                    map.SetTile(v, null);
                v.y++;
            }
            v.x++;
        }
    }

    private bool ShouldBeJar(Vector3Int v)
    {
        if (map.GetTile(v + Vector3Int.down) == platformTile)
            return true;
        if (map.GetTile(v + Vector3Int.up) == platformTile)
            return true;
        if (map.GetTile(v + Vector3Int.up * 2) == platformTile)
            return true;
        return false;
    }

    public void PrintToLog()
    {
        Debug.Log(gameObject.name);
    }
}
