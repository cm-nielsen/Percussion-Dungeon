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

    public float progress { get { return Mathf.Min((float)prog / progMax, 1); } }
    public int maxCount, minCount, rolls, floodDivision;
    public bool spawnUpgrade;

    private Tilemap map;
    private List<Room> roomSet;
    private List<PotentialRoom> rooms;
    private List<Vector3Int> occupiedPositions;

    private int rollCount = 0;

    private List<BoundsInt> floods = new List<BoundsInt>();
    private int roomIndex = 0, roomRowIndex = 0, rowsPerFrame = 2, prog = 0, progMax = 1;
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

            Vector3Int v = -((Vector3Int)room.size / 2);
            v.x += row;

            BoundsInt bound = new BoundsInt(v, new Vector3Int(1, room.size.y, 1));
            //Debug.Log(bound);
            TileBase[] colTiles = room.map.GetTilesBlock(bound);
            for (int i = 0; i < colTiles.Length; i++)
            {
                if (!tiles.Contains(colTiles[i]))
                    colTiles[i] = null;
            }
            map.SetTilesBlock(new BoundsInt(v + pos, new Vector3Int(1, room.size.y, 1)), colTiles);
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
        if (roomIndex < 0)
        {
            foreach (Vector3Int v in occupiedPositions)
                FloodRoomBorders(v);
            progMax = floods.Count + (rooms.Count * size.x) + 2;
            roomIndex++;
        }
        if(roomIndex < rooms.Count)
        {
            // fill rooms column by column in dynamic batches
            for (int i = 0; i < rowsPerFrame; i++)
            {
                if (rooms[roomIndex].FillColumn(map, 
                    new TileBase[] { platform, jar, enemy }, roomRowIndex++))
                {
                    roomIndex++;
                    roomRowIndex = 0;
                    break;
                }
                prog++;
            }
            if (Time.unscaledDeltaTime > 1 / 16.0 && rowsPerFrame > 1)
                rowsPerFrame = rowsPerFrame / 2;
            else
                rowsPerFrame++;

        }else if(roomIndex == rooms.Count)
        {
            // fill generated edges in dynamic batches
            BoundsInt bound;
            TileBase[] fillTiles;

            for (int i = 0; i < rowsPerFrame; i++)
            {
                bound = floods[roomRowIndex++];
                prog++;

                fillTiles = new TileBase[bound.size.x * bound.size.y];
                for (int j = 0; j < fillTiles.Length; j++)
                    fillTiles[j] = platform;
                map.SetTilesBlock(bound, fillTiles);
                if (roomRowIndex >= floods.Count)
                {
                    roomIndex++;
                    return;
                }
            }

            if (Time.unscaledDeltaTime > 1 / 16.0 && rowsPerFrame > 1)
                rowsPerFrame = rowsPerFrame / 2;
            else
                rowsPerFrame++;
        }
        else if(roomIndex == rooms.Count + 1)
        {
            prog++;
            // fill in remaining key stage elements
            int rand = Random.Range(rooms.Count / 2, rooms.Count);
            PotentialRoom gateRoom = rooms[rand];
            gateRoom.Fill(map, gate);
            if (spawnUpgrade)
            {
                int rand2 = Random.Range(rooms.Count / 2, rooms.Count);
                while (rand2 == rand)
                    rand2 = Random.Range(rooms.Count / 2, rooms.Count);
                PotentialRoom upgradeRoom = rooms[rand2];
                upgradeRoom.Fill(map, upgrade);
            }
            //rooms[0].Fill(map, spawn);
            roomIndex++;
        }else if(roomIndex == rooms.Count + 2)
        {
            TileBase t = platform.sibling;
            Vector3Int v = new Vector3Int(0, size.y / 2, 0);
            while (map.GetTile(v) == platform || map.GetTile(v + Vector3Int.right) == platform ||
                map.GetTile(v + Vector3Int.left) == platform)
                v.y--;
            v.y++;

            BoundsInt bound = new BoundsInt(new Vector3Int(-1, v.y, 0),
                new Vector3Int(3, overflowSize.y + ((size.y / 2) - v.y), 1));
            TileBase[] tiles = new TileBase[bound.size.x * bound.size.y];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = t;
            map.SetTilesBlock(bound, tiles);

            LoadingScreen.loaded = true;
            Gate.loaded = true;
            roomIndex++;
        }
    }

    private void FloodRoomBorders(Vector3Int v)
    {
        if (!occupiedPositions.Contains(v + Vector3Int.right * size.x))
            FormVertFlood(v + Vector3Int.right * size.x);
        if (!occupiedPositions.Contains(v + Vector3Int.left * size.x))
            FormVertFlood(v + Vector3Int.left * size.x, true);
        if (!occupiedPositions.Contains(v + Vector3Int.up * size.y))
            FormHorFlood(v + Vector3Int.up * size.y);
        if (!occupiedPositions.Contains(v + Vector3Int.down * size.y))
            FormHorFlood(v + Vector3Int.down * size.y, true);

        Vector3Int s = (Vector3Int)size;
        Vector3Int s2 = s;
        s2.x = -s.x;
        if (!occupiedPositions.Contains(v + s))
            FormCornerFlood(v + s);
        if (!occupiedPositions.Contains(v - s))
            FormCornerFlood(v - s, true, true);
        if (!occupiedPositions.Contains(v + s2))
            FormCornerFlood(v + s2, true);
        if (!occupiedPositions.Contains(v - s2))
            FormCornerFlood(v - s2, false, true);
    }

    private void FormHorFlood(Vector3Int v, bool down = false)
    {
        int max = v.x + size.x / 2;
        v -= (Vector3Int)size / 2;
        if (down)
            v.y += size.y - overflowSize.y;

        Vector3Int bound = new Vector3Int(size.x / floodDivision, overflowSize.y, 1);

        do
        {
            floods.Add(new BoundsInt(v, bound));
            v.x += size.x / floodDivision;
        } while (v.x < max);
    }

    private void FormVertFlood(Vector3Int v, bool right = false)
    {
        int max = v.y + size.y / 2;
        v -= (Vector3Int)size / 2;
        if (right)
            v.x += size.x - overflowSize.x;

        Vector3Int bound = new Vector3Int(overflowSize.x, size.y / floodDivision, 1);

        do
        {
            floods.Add(new BoundsInt(v, bound));
            v.y += size.y / floodDivision;
        } while (v.y < max);
    }

    private void FormCornerFlood(Vector3Int v, bool right = false, bool down = false)
    {
        v -= (Vector3Int)size / 2;
        if (right)
            v.x += size.x - overflowSize.x;
        if (down)
            v.y += size.y - overflowSize.y;
        floods.Add(new BoundsInt(v, new Vector3Int(overflowSize.x, overflowSize.y, 1)));
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
        //map.ClearAllTiles();
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
        roomIndex = -1;
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
}
