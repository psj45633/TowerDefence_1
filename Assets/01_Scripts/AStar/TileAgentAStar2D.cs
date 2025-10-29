using System.Collections.Generic;
using UnityEngine;

public class TileAgentAStar2D : MonoBehaviour
{
    public static event System.Action RepathAllRequested;

    public static void RequestRepathAll()
    {
        RepathAllRequested?.Invoke();
    }

    [Header("Refs")]
    public PathGrid2D grid;
    public Transform goal;

    [Header("Move")]
    public float speed = 2f;
    public float arriveEps = 0.05f;
    public float repathIntervel = 0.25f;
    public bool allowDiagonal = true;
    public bool cornerCutBlock = true;

    Rigidbody2D rb;
    readonly List<Vector2Int> path = new();
    int idx = -1;

    static readonly Vector2Int[] DIR4 = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    static readonly Vector2Int[] DIR8 = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 코너에서 매끈
    }

    public void Init(PathGrid2D g,Transform goal)
    {
        if (grid) grid.OnGridChanged -= Repath;

        grid = g;
        this.goal = goal;

        if (!grid || !this.goal)
        {
            Debug.LogError("[Agent] grid/goal null. Init 실패");
            return;
        }
        grid.OnGridChanged += Repath;

        // 풀에서 나온 경우를 대비해 상태 초기화
        rb.linearVelocity = Vector2.zero;
        path.Clear();
        idx = -1;

        Repath();
    }

    private void OnEnable() => RepathAllRequested += Repath;
    private void OnDisable() => RepathAllRequested -= Repath;


    private void OnDestroy() { if (grid) grid.OnGridChanged -= Repath; }

    private void Start() => Repath();

    private void FixedUpdate()
    {
        if (!goal || idx < 0 || idx >= path.Count)
        { rb.linearVelocity = Vector2.zero; return; }

        var nextCell = path[idx];
        var nextCenter = grid.CellCenterWorld(nextCell);

        Vector3 pos = transform.position;
        Vector3 newPos = Vector3.MoveTowards(pos, nextCenter, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        float dist = (nextCenter - newPos).sqrMagnitude;

        if (dist <= arriveEps * arriveEps)
        {
            rb.MovePosition(nextCenter); // 코너에서 정확 스냅
            idx++;
            return;
        }
        //if (!goal || idx < 0 || idx >= path.Count) { rb.linearVelocity = Vector2.zero; return; }

        //var nextCell = path[idx];
        //var nextCenter = grid.CellCenterWorld(nextCell);

        //Vector3 pos = transform.position;
        //Vector3 to = nextCenter - pos;
        //float dist = to.magnitude;
        //float step = speed * Time.fixedDeltaTime;

        ////  이동하기 전에: step 범위(±여유)면 바로 스냅 + 다음 노드
        //if (dist <= Mathf.Max(arriveEps, step * 1.1f))
        //{
        //    rb.MovePosition(nextCenter);
        //    rb.linearVelocity = Vector2.zero;
        //    idx++;
        //    return;
        //}

        //// 그 외에는 한 프레임만 전진
        //Vector3 newPos = pos + to.normalized * step;
        //rb.MovePosition(newPos);
    }

    public void Repath()
    {
        if (!grid || !goal) return;
        var s = grid.WorldToCell(transform.position);
        var g = grid.WorldToCell(goal.position);

        if (FindPath(s, g, path))
        {
            if (allowDiagonal) CompressPath8(path);
            idx = (path.Count >= 2 && path[0] == s) ? 1 : 0;
        }
        else { idx = -1; rb.linearVelocity = Vector2.zero; }
    }

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
                p[write++] = p[i - 1];   // 꺾이는 지점만 남김
                prevDir = dir;
            }
        }
        p[write++] = p[^1];               // 마지막 목적지
        p.RemoveRange(write, p.Count - write);
    }

    bool FindPath(Vector2Int start, Vector2Int goal, List<Vector2Int> outPath)
    {
        outPath.Clear();
        if (!grid.InBounds(start) || !grid.InBounds(goal) || !grid.IsWalkable(start) || !grid.IsWalkable(goal))
            return false;

        var open = new MinHeap();
        var came = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        int H(Vector2Int a)
        {
            int dx = Mathf.Abs(a.x - goal.x), dy = Mathf.Abs(a.y - goal.y);
            return allowDiagonal ? 10 * (dx + dy) + (14 - 20) * Mathf.Min(dx, dy) : 10 * (dx + dy);
        }

        open.Push(start, H(start));
        var dirs = allowDiagonal ? DIR8 : DIR4;

        while(open.Count > 0)
        {
            var cur = open.Pop();
            if(cur == goal) { Reconstruct(came, cur, outPath); return true; }

            foreach(var d in dirs)
            {
                var nb = cur + d;
                if(!grid.InBounds(nb) || !grid.IsWalkable(nb)) continue;

                bool diag = (d.x != 0 && d.y != 0);
                if( allowDiagonal && cornerCutBlock && diag)
                {
                    var sideA = new Vector2Int(cur.x + d.x, cur.y);
                    var sideB = new Vector2Int(cur.x, cur.y + d.y);
                    if(!grid.IsWalkable(sideA) || !grid.IsWalkable(sideB)) continue;
                }

                int step = diag ? 14 : 10;
                int tentative = gScore[cur] + step;

                if(!gScore.TryGetValue(nb, out int old) || tentative < old)
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
        while(came.TryGetValue(cur,out var prev)) { cur= prev; outPath.Add(cur); }
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

    private void OnDrawGizmosSelected()
    {
        if (!grid || path == null) return;
        if (path.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(grid.CellCenterWorld(path[i]), grid.CellCenterWorld(path[i + 1]));
    }


}
