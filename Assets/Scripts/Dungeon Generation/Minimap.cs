using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Minimap : MonoBehaviour
{
    public Sprite pPos, roomNode, connection;
    //public Material platformMat, playerMat;
    public float nodeDistance;

    private List<GameObject> nodes = new List<GameObject>();
    private List<Vector2> nodePositions = new List<Vector2>(),
        bridges = new List<Vector2>();
    private Draggable nodeParent;
    private SpriteRenderer pPointer;
    private ControlKey con;
    private Transform player;
    private Vector2 refPos = Vector2.zero;
    private Vector2 roomSize;

    private bool valid = false, active = false, blink = false;
    // Start is called before the first frame update
    void Awake()
    {
        con = GetComponent<ControlKey>();

        nodeParent = Instantiate(new GameObject(), transform).AddComponent<Draggable>();
        nodeParent.name = "Map";

        GameObject pPointerObj = Instantiate(new GameObject(), nodeParent.transform);
        pPointer = pPointerObj.AddComponent<SpriteRenderer>();
        pPointer.sprite = pPos;
        pPointer.sortingLayerName = "UI";
        //s.material = playerMat;

        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            r.enabled = active;

        Music.onBeat.Add(OnBeat);

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
        if (active != con["map"])
        {
            if(Time.timeScale == 0)
            {
                active = false;
                return;
            }
            active = con["map"];
            foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
                r.enabled = active;
        }

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

    public void OnRoomEnter(Vector2 dir)
    {
        if (dir != Vector2.zero)
            MakeBridge(dir);

        refPos += roomSize * dir;
        Vector2 nodePos = (refPos / roomSize) * nodeDistance;
        pPointer.transform.localPosition = nodePos;
        transform.localPosition = -(Vector3)nodePos + Vector3.forward * 10;

        if (nodePositions.Any(x => x == nodePos))
            return;

        GameObject g = Instantiate(new GameObject(), nodeParent.transform);
        g.transform.localPosition = nodePos;
        SpriteRenderer s = g.AddComponent<SpriteRenderer>();
        s.sprite = roomNode;
        s.sortingLayerName = "UI";
        s.enabled = active;
        //s.material = platformMat;

        nodes.Add(g);
        nodePositions.Add(nodePos);
        nodeParent.ExpandLimits(nodePos);
    }

    public void MakeBridge(Vector2 dir)
    {
        Vector2 pos = ((refPos + roomSize * dir / 2) / roomSize) * nodeDistance;

        if (bridges.Any(x => x == pos))
            return;

        GameObject g = Instantiate(new GameObject(), nodeParent.transform);
        g.transform.localPosition = pos;
        SpriteRenderer s = g.AddComponent<SpriteRenderer>();
        s.sprite = connection;
        s.sortingLayerName = "UI";
        s.enabled = active;
        if (dir.x == 0)
            g.transform.localRotation = Quaternion.Euler(0, 0, 90);

        bridges.Add(pos);
    }

    private void OnBeat()
    {
        blink = !blink;
        if (pPointer)
            pPointer.enabled = active && blink;
    }
}
