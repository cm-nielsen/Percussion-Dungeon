using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap map;
    public TileBase tile;
    public GameObject[] roomSetObjects;

    public int width, height, maxCount, minCount, rolls;

    private List<Room> roomSet;
    private List<PotentialRoom> rooms;
    private List<Vector3Int> occupiedPositions;

    private int rollCount = 0;
    private class PotentialRoom
    {
        public Room room;
        public Vector3Int pos;
        public bool branched = false;
        public PotentialRoom(Room r, Vector3Int v)
        {
            room = r;
            pos = v;
        }
        public void Fill(Tilemap map)
        {
            Vector3Int v = pos - ((Vector3Int)room.size / 2);
            Debug.Log(pos);
            for(int i = 0; i < room.size.x; i++)
            {
                v.y = pos.y - (room.size.y / 2);
                for(int j = 0; j < room.size.y; j++)
                {
                    map.SetTile(v, room.map.GetTile(v - pos));
                    v.y++;
                    Debug.Log(v);
                }
                v.x++;
            }
        }
    }

    private Vector2 roomSize;

    // Start is called before the first frame update
    void Start()
    {
        roomSet = new List<Room>();
        foreach(GameObject g in roomSetObjects)
        {
            Room r = g.GetComponentInChildren<Room>();
            if (r && !roomSet.Contains(r))
            {
                r.UpdateAttributes();
                roomSet.Add(r);
            }
        }

        GenerateLevel();
    }

    private List<Room> GetRoomsOfType(Doors doors)
    {
        return roomSet.Where(x => 
            ((x.doors & doors) == doors)).ToList();
    }

    private List<Room> GetRoomsOfType(Doors doors, Doors walls)
    {
        return roomSet.Where(x =>
        ((x.doors & doors) == doors) && ((x.doors & walls) == Doors.none)).ToList();
    }

    private PotentialRoom MakeRoom(Doors doors, Vector3Int pos)
    {
        List<Room> ls = roomSet.Where(x =>
            ((x.doors & doors) == doors)).ToList();
        if (ls.Count < 1)
            return null;

        return new PotentialRoom(ls[Random.Range(0, ls.Count)], pos);
    }

    private PotentialRoom MakeRoom(Doors doors, Doors walls, Vector3Int pos)
    {
        List<Room> ls = roomSet.Where(x =>
        ((x.doors & doors) == doors) && ((x.doors & walls) == Doors.none)).ToList();
        if (ls.Count < 1)
            return null;

        return new PotentialRoom(ls[Random.Range(0, ls.Count)], pos);
    }

    private bool AddRoom(PotentialRoom pr)
    {
        if (occupiedPositions.Contains(pr.pos))
            return false;
        rooms.Add(pr);
        occupiedPositions.Add(pr.pos);
        return true;
    }

    public void GenerateLevel()
    {
        rooms = new List<PotentialRoom>();
        occupiedPositions = new List<Vector3Int>();
        map.ClearAllTiles();
        Vector3Int pos = Vector3Int.zero;
        //intitial room
        AddRoom(MakeRoom(Doors.down, Doors.up, pos));

        while (true)
        {
            bool fin = true;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (!rooms[i].branched)
                {
                    Branch(rooms[i]);
                    fin = false;
                }
                if (rooms.Count >= maxCount)
                    break;
            }

            if (fin)
                break;
        }

        if(rooms.Count < minCount && rollCount < rolls)
        {
            rollCount++;
            GenerateLevel();
            return;
        }
        FillRooms();
    }

    private void Branch(PotentialRoom pr)
    {
        if ((pr.room.doors & Doors.left) != 0)
            AddRoom(MakeRoom(Doors.right, Doors.up, pr.pos + Vector3Int.left * pr.room.size.x));
        if ((pr.room.doors & Doors.right) != 0)
            AddRoom(MakeRoom(Doors.left, Doors.up, pr.pos + Vector3Int.right * pr.room.size.x));
        if ((pr.room.doors & Doors.down) != 0)
            AddRoom(MakeRoom(Doors.up, pr.pos + Vector3Int.down * pr.room.size.y));

        pr.branched = true;
    }

    private void FillRooms()
    {
        foreach(PotentialRoom r in rooms)
        {
            r.Fill(map);
        }
    }
}
