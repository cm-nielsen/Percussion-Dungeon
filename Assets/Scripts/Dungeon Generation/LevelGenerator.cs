using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> roomSetObjects;
    public RuleTile platform, jar, enemy, gate, spawn, upgrade;
    public Vector2Int overflowSize;
    public Vector2Int size { get { return roomSetObjects[0].GetComponentInChildren<Room>().size; } }

    public int maxCount, minCount, rolls;
    public bool spawnUpgrade;

    private Tilemap map;
    private List<Room> roomSet;
    private List<PotentialRoom> rooms;
    private List<Vector3Int> occupiedPositions;

    private int rollCount = 0;

    private EdgeCollider2D edgeguard;
    private int roomIndex = 0, roomRowIndex = 0, rowsPerFrame = 5;
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
        public void Fill(Tilemap map, TileBase tile)
        {
            Vector3Int v = pos - ((Vector3Int)room.size / 2);
            //Debug.Log(pos);
            for(int i = 0; i < room.size.x; i++)
            {
                v.y = pos.y - (room.size.y / 2);
                for(int j = 0; j < room.size.y; j++)
                {
                    TileBase t = room.map.GetTile(v - pos);
                    if (t == tile)
                        map.SetTile(v, room.map.GetTile(v - pos));
                    v.y++;
                    //Debug.Log(v);
                }
                v.x++;
            }
        }
        public void Fill(Tilemap map, TileBase[] tiles)
        {
            Vector3Int v = pos - ((Vector3Int)room.size / 2);
            //Debug.Log(pos);
            for (int i = 0; i < room.size.x; i++)
            {
                v.y = pos.y - (room.size.y / 2);
                for (int j = 0; j < room.size.y; j++)
                {
                    TileBase t = room.map.GetTile(v - pos);
                    if (tiles.Contains(t))
                        map.SetTile(v, room.map.GetTile(v - pos));
                    v.y++;
                    //Debug.Log(v);
                }
                v.x++;
            }
        }
        public bool FillColumn(Tilemap map, TileBase[] tiles, int row)
        {
            if (row >= room.size.x)
                return true;
            Vector3Int v = pos - ((Vector3Int)room.size / 2);
            v.x += row;

            int i = row;
            v.y = pos.y - (room.size.y / 2);
            for (int j = 0; j < room.size.y; j++)
            {
                TileBase t = room.map.GetTile(v - pos);
                if (tiles.Contains(t))
                    map.SetTile(v, room.map.GetTile(v - pos));
                v.y++;
                //Debug.Log(v);
            }
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponentInChildren<Tilemap>();

        if (roomSetObjects.Count < 1)
            return;
        Vector2Int size = roomSetObjects[0].GetComponentInChildren<Room>().size;

        roomSet = new List<Room>();
        foreach(GameObject g in roomSetObjects)
        {
            Room r = g.GetComponentInChildren<Room>();
            if (r && !roomSet.Contains(r) && r.size == size)
            {
                r.UpdateAttributes();
                roomSet.Add(r);
            }
        }

        GenerateLevel();
    }

    private void Update()
    {

        if(roomIndex < rooms.Count)
        {
            for (int i = 0; i < rowsPerFrame; i++)
            {
                if (rooms[roomIndex].FillColumn(map, new TileBase[] { platform, jar, enemy }, roomRowIndex++))
                {
                    roomIndex++;
                    roomRowIndex = 0;
                    break;
                }
            }
            if (Time.deltaTime > 1 / 16.0 && rowsPerFrame > 1)
                rowsPerFrame--;
            else
                rowsPerFrame++;

            //rooms[roomIndex].Fill(map, new TileBase[] { platform, jar, enemy });
            //roomIndex++;
        }else if(roomIndex == rooms.Count)
        {
            Vector3Int min = Vector3Int.zero;
            foreach (Vector2Int v in occupiedPositions)
            {
                if (v.x < min.x)
                    min.x = v.x;
                if (v.y < min.y)
                    min.y = v.y;
            }
            map.FloodFill(min - (Vector3Int)overflowSize, platform);
            
            roomIndex++;
        }else if(roomIndex == rooms.Count + 1)
        {
            Vector3Int max = Vector3Int.zero;
            foreach (Vector2Int v in occupiedPositions)
            {
                if (v.x > max.x)
                    max.x = v.x;
                if (v.y > max.y)
                    max.y = v.y;
            }
            map.FloodFill(max + (Vector3Int)overflowSize, platform);

            roomIndex++;
        }
        else if(roomIndex == rooms.Count + 2)
        {
            int rand = Random.Range(rooms.Count / 2, rooms.Count), rand2 = 0;
            PotentialRoom gateRoom = rooms[rand];
            gateRoom.Fill(map, gate);
            if (spawnUpgrade)
            {
                rand2 = Random.Range(rooms.Count / 2, rooms.Count);
                while (rand2 == rand)
                    rand2 = Random.Range(rooms.Count / 2, rooms.Count);
                PotentialRoom upgradeRoom = rooms[rand2];
                upgradeRoom.Fill(map, upgrade);
            }
            Destroy(edgeguard);
            roomIndex++;
        }
    }

    private PotentialRoom MakeRoom(Doors doors, Vector3Int pos)
    {
        List<Room> ls = roomSet.Where(x =>
            ((x.doors & doors) == doors)).ToList();
        if (ls.Count < 1)
        {
            Debug.Log("[Level Generation]\nNo suitable room found. Set may not be comprehensive.");
            return null;
        }

        return new PotentialRoom(ls[Random.Range(0, ls.Count)], pos);
    }

    private PotentialRoom MakeRoom(Doors doors, Doors walls, Vector3Int pos)
    {
        List<Room> ls = roomSet.Where(x =>
        ((x.doors & doors) == doors) && ((x.doors & walls) == Doors.none)).ToList();
        if (ls.Count < 1)
        {
            Debug.Log("[Level Generation]\nNo suitable room found. Set may not be comprehensive.");
            return null;
        }

        return new PotentialRoom(ls[Random.Range(0, ls.Count)], pos);
    }

    private Room FindRoomOfType(Doors doors, Doors walls)
    {
        List<Room> ls = roomSet.Where(x =>
        ((x.doors & doors) == doors) && ((x.doors & walls) == Doors.none)).ToList();
        if (ls.Count < 1)
        {
            Debug.Log("[Level Generation]\nNo suitable room found. Set may not be comprehensive." +
                "doors: " + doors.ToString() + ", walls: " + walls);
            return null;
        }

        return ls[Random.Range(0, ls.Count)];
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

        foreach (PotentialRoom pr in rooms)
        {
            Close(pr);
        }
        //FillRooms();
        rooms[roomIndex++].Fill(map, new TileBase[] { platform, jar, enemy });
        rooms[0].Fill(map, spawn);
        edgeguard = gameObject.AddComponent<EdgeCollider2D>();
        Vector2 v = (Vector2)rooms[0].room.size / 8f;
        Vector2[] ar = new Vector2[5];
        ar[0] = ar[4] = v;
        ar[1] = new Vector2(v.x, -v.y);
        ar[2] = new Vector2(-v.x, -v.y);
        ar[3] = new Vector2(-v.x, v.y);
        edgeguard.points = ar;
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

    private bool Close(PotentialRoom pr)
    {
        bool changed = false;
        Vector3Int v = pr.pos + Vector3Int.left * pr.room.size.x;
        changed |= CloseHelper(pr, v, Doors.left, Doors.right);
        v.x += pr.room.size.x * 2;
        changed |= CloseHelper(pr, v, Doors.right, Doors.left);
        v = pr.pos + Vector3Int.down * pr.room.size.y;
        return changed | CloseHelper(pr, v, Doors.down, Doors.up);
    }

    private bool CloseHelper(PotentialRoom pr, Vector3Int v, Doors open, Doors opp)
    {
        if ((pr.room.doors & open) == 0)
            return false;

        if (occupiedPositions.Contains(v))
        {
            PotentialRoom connected = FindPotRoomAtPos(v);
            if ((connected.room.doors & opp) == 0)
            {
                Doors newDoors = connected.room.doors | opp;
                connected.room = FindRoomOfType(newDoors, ~newDoors);
                return true;
            }
            return false;
        }
        else
        {
            Doors newDoors = pr.room.doors & ~open;
            pr.room = FindRoomOfType(newDoors, ~newDoors);
            return true;
        }
    }

    private PotentialRoom FindPotRoomAtPos(Vector3Int v)
    {
        foreach (PotentialRoom pr in rooms)
            if (pr.pos == v)
                return pr;
        return null;
    }

    private void FillRooms()
    {
        foreach(PotentialRoom r in rooms)
        {
            r.Fill(map, new TileBase[] { platform, jar, enemy });
        }

        Vector3Int min = Vector3Int.zero, max = Vector3Int.zero;
        foreach(Vector2Int v in occupiedPositions)
        {
            if (v.x < min.x)
                min.x = v.x;
            if (v.y < min.y)
                min.y = v.y;

            if (v.x > max.x)
                max.x = v.x;
            if (v.y > max.y)
                max.y = v.y;
        }
        map.FloodFill(min - (Vector3Int)overflowSize, platform);
        map.FloodFill(max + (Vector3Int)overflowSize, platform);

        rooms[0].Fill(map, spawn);

        int rand = Random.Range(rooms.Count / 2, rooms.Count), rand2 = 0;
        PotentialRoom gateRoom = rooms[rand];
        gateRoom.Fill(map, gate);
        if (spawnUpgrade)
        {
            rand2 = Random.Range(rooms.Count / 2, rooms.Count);
            while (rand2 == rand)
                rand2 = Random.Range(rooms.Count / 2, rooms.Count);
            PotentialRoom upgradeRoom = rooms[rand2];
            upgradeRoom.Fill(map, upgrade);
        }
    }
}
