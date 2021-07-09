using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> roomSetObjects;
    public RuleTile platform, jar, enemy, gate, upgrade;
    public TileBase upgradeRoomTile;
    public Vector2Int overflowSize;
    public Vector2Int size { get { return roomSetObjects[0].GetComponentInChildren<Room>().size; } }

    public float progress { get { return Mathf.Min((float)batchIndex / tiles.Length, 1); } }
    public int maximumRooms, minimumRooms, rolls, minimumFrameRate = 16;
    public bool spawnUpgrade;

    // for room generation
    private Tilemap map;
    private List<Room> roomSet;
    private List<PotentialRoom> rooms;
    private List<Vector3Int> occupiedPositions;

    private int rollCount = 0, upgradeRoomIndex;

    // for tile placement
    private TileBase[] tiles;
    private List<TileBase> tileList;
    private Vector3Int[] positions;
    private List<Vector3Int> positionList;

    private int batchIndex = 0, batchCount = 400;

    private float startTime;


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

        // transfers all tiles of specified type into the given tilemap
        public void Fill(Tilemap map, TileBase tile)
        {
            Vector3Int v = pos - ((Vector3Int)room.size / 2);
            Vector3Int dif;
            Tilemap sourceMap = room.map;
            int sizeX = room.size.x;
            int sizeY = room.size.y;
            int y = pos.y - (sizeY / 2);

            for (int i = 0; i < sizeX; i++)
            {
                v.y = y;
                for(int j = 0; j < sizeY; j++)
                {
                    dif = v - pos;
                    TileBase t = sourceMap.GetTile(dif);
                    if (t == tile)
                        map.SetTile(v, sourceMap.GetTile(dif));
                    v.y++;
                }
                v.x++;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
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
        GenerateTileArray();
    }

    private void Update()
    {
        if(batchIndex < tiles.Length)
        {
            // copy tiles from constructed array in dynamic batches
            if (batchIndex + batchCount > tiles.Length)
                batchCount = tiles.Length - batchCount;// - 1;
            map.SetTiles(positions.Skip(batchIndex).Take(batchCount).ToArray(),
                tiles.Skip(batchIndex).Take(batchCount).ToArray());

            batchIndex += batchCount;

            if (Time.unscaledDeltaTime * minimumFrameRate > 1 && batchCount > 10)
                batchCount = (batchCount * 3) / 4;
                //print("batchCount decreased, timestep: " + Time.unscaledDeltaTime * minimumFrameRate);
            else
                batchCount = batchCount * 9 / 8;
        }
        else
        {
            // Spawn Gate and Upgrade
            int rand = upgradeRoomIndex;
            if (spawnUpgrade)
            {
                rooms[upgradeRoomIndex].Fill(map, upgrade);
                while (rand == upgradeRoomIndex)
                    rand = Random.Range(rooms.Count / 2, rooms.Count);
            }
            rooms[rand].Fill(map, gate);

            // drill hole from starting room to hub
            List<Vector3Int> vList = new List<Vector3Int>();

            Vector3Int v = new Vector3Int(0, size.y / 2, 0);
            v.y--;
            while (map.GetTile(v) == platform || map.GetTile(v + Vector3Int.right) == platform ||
                map.GetTile(v + Vector3Int.left) == platform)
            {
                vList.Add(v);
                vList.Add(v + Vector3Int.right);
                vList.Add(v + Vector3Int.left);
                v.y--;
            }

            TileBase[] tiles = new TileBase[vList.Count];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = null;// platform.sibling;
            map.SetTiles(vList.ToArray(), tiles);

            // wall the sides of the entry hole
            vList.Clear();
            v.y = size.y / 2;
            v.x = 2;
            while (map.GetTile(v) != platform)
            {
                vList.Add(v);
                vList.Add(v + Vector3Int.left * 4);
                v.y++;
            }
            tiles = new TileBase[vList.Count];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = platform;
            map.SetTiles(vList.ToArray(), tiles);

            // finalize loading status
            LoadingScreen.loaded = true;
            Gate.loaded = true;
            //print("Level Generation Finished in " + (Time.time - startTime) + " seconds");
            enabled = false;
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
        int maxX = v.x + size.x / 2;
        int maxY = v.y - size.y / 2 + overflowSize.y;
        v -= (Vector3Int)size / 2;
        int x = v.x;
        if (down)
        {
            maxY += size.y - overflowSize.y;
            v.y += size.y - overflowSize.y;
        }

        QueueRectFill(v, maxX, maxY, platform.sibling);
    }

    private void FormVertFlood(Vector3Int v, bool right = false)
    {
        int maxX = v.x - this.size.x / 2 + this.overflowSize.x;
        int maxY = v.y + this.size.y / 2;
        v -= (Vector3Int)size / 2;
        if (right)
        {
            maxX += size.x - overflowSize.x;
            v.x += size.x - overflowSize.x;
        }

        QueueRectFill(v, maxX, maxY, platform.sibling);
    }

    private void FormCornerFlood(Vector3Int v, bool right = false, bool down = false)
    {
        int maxX = v.x - this.size.x / 2 + this.overflowSize.x;
        int maxY = v.y - this.size.y / 2 + this.overflowSize.y;
        v -= (Vector3Int)size / 2;
        if (down)
        {
            maxY += size.y - overflowSize.y;
            v.y += size.y - overflowSize.y;
        }
        if (right)
        {
            maxX += size.x - overflowSize.x;
            v.x += size.x - overflowSize.x;
        }
        QueueRectFill(v, maxX, maxY, platform.sibling);
    }

    private void QueueRectFill(Vector3Int startPos, int maxX, int maxY, TileBase t)
    {
        int x = startPos.x;
        while (startPos.y < maxY)
        {
            startPos.x = x;
            while (startPos.x < maxX)
            {
                tileList.Add(t);
                positionList.Add(startPos);
                startPos.x++;
            }
            startPos.y++;
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
                if (rooms.Count >= maximumRooms)
                    break;
            }

            if (fin)
                break;
        }

        if(rooms.Count < minimumRooms && rollCount < rolls)
        {
            rollCount++;
            GenerateLevel();
            return;
        }

        foreach (PotentialRoom pr in rooms)
        {
            Close(pr);
        }

        upgradeRoomIndex = Random.Range(rooms.Count / 2, rooms.Count);
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

    private void GenerateTileArray()
    {
        tileList = new List<TileBase>();
        positionList = new List<Vector3Int>();
        TileBase[] filter = new TileBase[] { platform, platform.sibling, enemy, jar, upgradeRoomTile };

        foreach (PotentialRoom r in rooms)
            r.room.FetchTilePositionSet(filter, r.pos).WriteTiles(tileList, positionList);

        TileBase[] replaceWithPlatform = new TileBase[] { platform.sibling, upgradeRoomTile };
        for (int i = 0; i < tileList.Count; i++)
            if (replaceWithPlatform.Contains(tileList[i]))
                tileList[i] = platform;

        rooms[upgradeRoomIndex].room.FetchTilePositionSet(new TileBase[] { platform.sibling, upgradeRoomTile },
            rooms[upgradeRoomIndex].pos).SetTypeNull(upgradeRoomTile).WriteTiles(tileList, positionList);

        foreach (Vector3Int v in occupiedPositions)
            FloodRoomBorders(v);

        tiles = tileList.ToArray();
        positions = positionList.ToArray();
        batchIndex = 0;
    }
}
