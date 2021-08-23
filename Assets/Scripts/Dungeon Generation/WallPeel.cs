using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallPeel : MonoBehaviour
{
    public Tilemap map;
    public bool triggerOnLoad = true, triggerOnCollision;
    public PeelSection[] sections = new PeelSection[1];
    public AudioClip noise;

    public int[] speeds;

    private AudioClipPlayer sfx;
    private int index = 0, speed = 1;
    private bool go = false;

    // Start is called before the first frame update
    void Start()
    {
        sfx = GetComponent<AudioClipPlayer>();
        Initialize();
    }

    private void OnDrawGizmos()
    {
        Vector2 t = transform.position;

        foreach(PeelSection s in sections)
        {
            Gizmos.color = new Color(1, 0.5f, 1f, .5f);
            Gizmos.DrawSphere(t + s.start, .02f);
            Vector2 size = s.end - s.start;
            float w = s.width;
            if (map)
                w *= map.layoutGrid.cellSize.x;
            else
                w *= .25f;
            if (size.x == 0)
                size.x = w;
            if (size.y == 0)
                size.y = w;
            Gizmos.color = new Color(.5f, 0.5f, 1f, .3f);
            Gizmos.DrawCube(t + s.start + (s.end - s.start) / 2, size);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!go)
            return;
        if (triggerOnLoad && !Gate.loaded)
            return;

        if (index < speeds.Length)
            speed = speeds[index++];

        foreach (PeelSection s in sections)
            s.Update(speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggerOnCollision)
            return;
        if (collision.GetComponent<PlayerController>())
            go = true;

    }

    private void StartPeel()
    {
        go = true;
    }

    private void ForceUpdate(int n)
    {
        sfx.PlayClip(noise);
        foreach (PeelSection s in sections)
            s.Update(n);
    }

    public void Initialize()
    {
        if (map)
            foreach (PeelSection s in sections)
                s.Initialize(map, transform.position, noise, sfx);
    }

    [System.Serializable]
    public class PeelSection
    {
        private AudioClip noise;
        private AudioClipPlayer sfx;

        public Vector2 start, end;
        public int width, delay;

        private Tilemap map;
        private List<List<Vector3Int>> positions = new List<List<Vector3Int>>();

        private Vector3Int tri1 = Vector3Int.zero,
            tri2 = Vector3Int.zero;
        private int index, timer;

        public void Initialize(Tilemap m, Vector2 offset,
            AudioClip c = null, AudioClipPlayer fx = null)
        {
            noise = c;
            sfx = fx;

            map = m;
            Vector3Int dir = getDir();
            Vector3 normal = Quaternion.Euler(0, 0, 90) * dir * 1.5f;
            Vector3Int normali = new Vector3Int((int)normal.x, (int)normal.y, 0);
            tri1 -= dir;
            tri1 += normali;
            tri2 -= dir;
            tri2 -= normali;

            Vector3Int p = map.WorldToCell(start + offset);
            Vector3Int e = map.WorldToCell(end + offset);
            int radius = (width - 1) / 2;
            while (p.x != e.x || p.y != e.y)
            {
                List<Vector3Int> points = new List<Vector3Int>();
                points.Add(p);
                int i = 1;
                while (i <= radius)
                {
                    points.Add(p + tri1 * i);
                    points.Add(p + tri2 * i);
                    i++;
                }
                positions.Add(points);
                p += dir;
            }
            int w = radius - 1;
            while(w >= 0)
            {
                List<Vector3Int> points = new List<Vector3Int>();
                int i = 1;
                while (i <= radius)
                {
                    if (i >= radius - w)
                    {
                        points.Add(p + tri1 * i);
                        points.Add(p + tri2 * i);
                    }
                    i++;
                }
                positions.Add(points);
                p += dir;
                w--;
            }
        }

        public void Update()
        {
            if (timer++ < delay)
                return;

            if (index < positions.Count)
            {
                foreach (Vector3Int v in positions[index])
                    map.SetTile(v, null);
                if (noise && sfx)
                    sfx.PlayClip(noise);
            }
            index++;
        }

        public void Update(int n)
        {
            for (int i = 0; i < n; i++)
                Update();
        }

        private Vector3Int getDir()
        {
            Vector2 dir = end - start;
            dir = dir.normalized;

            if (dir.x > .5)
                dir.x = 1;
            else if (dir.x < -.5)
                dir.x = -1;
            else
                dir.x = 0;

            if (dir.y > .5)
                dir.y = 1;
            else if (dir.y < -.5)
                dir.y = -1;
            else
                dir.y = 0;

            return new Vector3Int((int)dir.x, (int)dir.y, 0);
        }
    }
}
