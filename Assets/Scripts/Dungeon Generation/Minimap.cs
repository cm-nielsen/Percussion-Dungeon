using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Minimap : MonoBehaviour
{
    public Sprite pPos, roomNode;
    //public Material platformMat, playerMat;
    public float nodeDistance;

    private List<GameObject> nodes = new List<GameObject>();
    private List<Vector2> nodePositions = new List<Vector2>();
    private GameObject pPointer;
    private ControlKey con;
    private Transform player;
    private Vector2 refPos = Vector2.zero;
    private Vector2 roomSize;

    private bool valid = false;
    // Start is called before the first frame update
    void Awake()
    {
        con = GetComponent<ControlKey>();
        pPointer = Instantiate(new GameObject(), transform);
        SpriteRenderer s = pPointer.AddComponent<SpriteRenderer>();
        s.sprite = pPos;
        s.sortingLayerName = "UI";
        //s.material = playerMat;

        valid = false;
        LevelGenerator lg = GameObject.FindObjectOfType<LevelGenerator>();
        if (lg == null)
            return;

        valid = true;
        roomSize = lg.size;
        roomSize /= 4f;
        OnRoomEnter(Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        bool active = con["map"];
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            r.enabled = active;

        if (!valid)
            return;

        if (player == null)
        {
            PlayerController pcon = GameObject.FindObjectOfType<PlayerController>();
            if (pcon)
                player = pcon.transform;
            else
                return;
        }

        Vector2 v = ((Vector2)player.position - refPos) * 2;
        if (v.x > roomSize.x)
            OnRoomEnter(Vector2.right);
        else if (v.x < -roomSize.x)
            OnRoomEnter(Vector2.left);
        else if (v.y > roomSize.y)
            OnRoomEnter(Vector2.up);
        else if (v.y < -roomSize.y)
            OnRoomEnter(Vector2.down);
    }

    private void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        nodes.Clear();
        nodePositions.Clear();

        valid = false;
        LevelGenerator lg = GameObject.FindObjectOfType<LevelGenerator>();
        if (lg == null)
            return;

        valid = true;
        roomSize = lg.size;
        roomSize /= 4f;
        OnRoomEnter(Vector2.zero);
    }

    public void OnRoomEnter(Vector2 dir)
    {
        refPos += roomSize * dir;
        Vector2 nodePos = (refPos / roomSize) * nodeDistance;
        pPointer.transform.localPosition = nodePos;

        if (nodePositions.Any(x => x == nodePos))
            return;

        GameObject g = Instantiate(new GameObject(), transform);
        g.transform.localPosition = nodePos;
        SpriteRenderer s = g.AddComponent<SpriteRenderer>();
        s.sprite = roomNode;
        s.sortingLayerName = "UI";
        //s.material = platformMat;

        nodes.Add(g);
        nodePositions.Add(nodePos);
    }
}
