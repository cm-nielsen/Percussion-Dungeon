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

    public int width, height, snakeCount, snakeHopLength, count;

    private List<Room> roomSet;
    private List<RoomPos> rooms;
    private class RoomPos
    {
        public Room room;
        public Vector3Int pos;
        public RoomPos(Room r, Vector3Int v) { room = r; pos = v; }
        public void Fill(Tilemap map)
        {
            Vector3Int v = pos - ((Vector3Int)room.size / 2);
            for(int i = 0; i < room.size.x; i++)
            {
                for(int j = 0; j < room.size.y; j++)
                {
                    map.SetTile(v, room.map.GetTile(v - pos));
                    v.y++;
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

        foreach (Room r in roomSet)
            r.PrintToLog();
        GenerateLevel();
    }

    private List<Room> GetRoomsOfType(Doors doors)
    {
        return roomSet.Where(x => ((x.doors & doors) == doors)).ToList();
    }

    public void GenerateLevel()
    {
        rooms = new List<RoomPos>();
        map.ClearAllTiles();
        Vector3Int pos = Vector3Int.zero;
        rooms.Add(new RoomPos(GetRoomsOfType(Doors.down)[0], pos));
        pos.y -= rooms[0].room.size.y;
        rooms.Add(new RoomPos(GetRoomsOfType(Doors.up)[0], pos));
        FillRooms();
    }

    private void FillRooms()
    {
        foreach(RoomPos r in rooms)
        {
            r.Fill(map);
        }
    }
}
