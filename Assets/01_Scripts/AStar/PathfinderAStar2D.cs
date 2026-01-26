using System.Collections.Generic;
using UnityEngine;

public class PathfinderAStar2D : MonoBehaviour
{
    public static event System.Action RepathAllRequested;
    public static void RequestRepathAll() => RepathAllRequested?.Invoke();

    [Header("Refs")]
    public PathGrid2D grid;
    public Transform goal;

    [Header("Options")]
    public bool allowDiagonal = true;
    public bool cornerCutBlock = true;
    public bool compressPath = true;

    static readonly Vector2Int[] DIR4 = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    static readonly Vector2Int[] DIR8 = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };

    private void Awake()
    {
        var spawner = GetComponent<EnemySpawner>();
        if (spawner) goal = spawner.goal;
    }

    private void Start()
    {
        RequestRepathAll();
    }

    private void OnEnable()
    {
        if (grid) grid.OnGridChanged += OnGridChanged;
    }

    private void OnDisable()
    {
        if (grid) grid.OnGridChanged -= OnGridChanged;
    }

    private void OnGridChanged() => RequestRepathAll();

    public bool TryBuildPathFromWorld(Vector3 startWorld, List<Vector2Int> outPath)
    {
        if (!grid || !goal) return false;

        var s = grid.WorldToCell(startWorld);
        var g = grid.WorldToCell(goal.position);

        if (!FindPath(s, g, outPath))
            return false;

        if (allowDiagonal && compressPath)
            CompressPath8(outPath);

        return true;
    }

    public Vector3 CellCenterWorld(Vector2Int cell) => grid.CellCenterWorld(cell);
    public Vector2Int WorldToCell(Vector3 world) => grid.WorldToCell(world);

    void CompressPath8(List<Vector2Int> p)
    {
        if (p.Count < 3) return;

        Vector2Int Clamp(Vector2Int d)
            => new Vector2Int(Mathf.Clamp(d.x, -1, 1), Mathf.Clamp(d.y, -1, 1));

        var prevDir = Clamp(p[1] - p[0]);
        int write = 1;

        for (int i = 2; i < p.Count; i++)
        {
            var dir = Clamp(p[i] - p[i - 1]);
            if (dir != prevDir)
            {
                p[write++] = p[i - 1];
                prevDir = dir;
            }
        }
        p[write++] = p[^1];
        p.RemoveRange(write, p.Count - write);
    }

    bool FindPath(Vector2Int start, Vector2Int goalCell, List<Vector2Int> outPath)
    {
        outPath.Clear();
        if (!grid.InBounds(start) || !grid.InBounds(goalCell) || !grid.IsWalkable(start) || !grid.IsWalkable(goalCell))
            return false;

        var open = new MinHeap();
        var came = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        int H(Vector2Int a)
        {
            int dx = Mathf.Abs(a.x - goalCell.x), dy = Mathf.Abs(a.y - goalCell.y);
            return allowDiagonal ? 10 * (dx + dy) + (14 - 20) * Mathf.Min(dx, dy) : 10 * (dx + dy);
        }

        open.Push(start, H(start));
        var dirs = allowDiagonal ? DIR8 : DIR4;

        while (open.Count > 0)
        {
            var cur = open.Pop();
            if (cur == goalCell) { Reconstruct(came, cur, outPath); return true; }

            foreach (var d in dirs)
            {
                var nb = cur + d;
                if (!grid.InBounds(nb) || !grid.IsWalkable(nb)) continue;

                bool diag = (d.x != 0 && d.y != 0);
                if (allowDiagonal && cornerCutBlock && diag)
                {
                    var sideA = new Vector2Int(cur.x + d.x, cur.y);
                    var sideB = new Vector2Int(cur.x, cur.y + d.y);
                    if (!grid.IsWalkable(sideA) || !grid.IsWalkable(sideB)) continue;
                }

                int step = diag ? 14 : 10;
                int tentative = gScore[cur] + step;

                if (!gScore.TryGetValue(nb, out int old) || tentative < old)
                {
                    gScore[nb] = tentative;
                    came[nb] = cur;
                    open.Push(nb, tentative + H(nb));
                }
            }
        }

        return false;
    }

    void Reconstruct(Dictionary<Vector2Int, Vector2Int> came, Vector2Int cur, List<Vector2Int> outPath)
    {
        outPath.Add(cur);
        while (came.TryGetValue(cur, out var prev)) { cur = prev; outPath.Add(cur); }
        outPath.Reverse();
    }

    class MinHeap
    {
        readonly List<(Vector2Int n, int f)> h = new();
        public int Count => h.Count;
        public void Push(Vector2Int n, int f) { h.Add((n, f)); Up(h.Count - 1); }
        public Vector2Int Pop() { var r = h[0].n; h[0] = h[^1]; h.RemoveAt(h.Count - 1); Down(0); return r; }
        int P(int i) => (i - 1) / 2; int L(int i) => i * 2 + 1; int R(int i) => i * 2 + 2;
        void Up(int i) { while (i > 0 && h[i].f < h[P(i)].f) { (h[i], h[P(i)]) = (h[P(i)], h[i]); i = P(i); } }
        void Down(int i) { for (; ; ) { int s = i, l = L(i), r = R(i); if (l < h.Count && h[l].f < h[s].f) s = l; if (r < h.Count && h[r].f < h[s].f) s = r; if (s == i) break; (h[i], h[s]) = (h[s], h[i]); i = s; } }
    }
}