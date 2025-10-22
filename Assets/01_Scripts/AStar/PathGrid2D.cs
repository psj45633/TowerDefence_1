using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGrid2D : MonoBehaviour
{
    [Header("Grid(월드기준)")]
    public Vector2 origin = new Vector2(-6, -11); // 좌하단
    public Vector2Int size = new Vector2Int(13, 24); // 셀 수
    public Vector2 cellSize = new Vector2(1, 1); //셀 크기

    [Header("Blocks")]
    public LayerMask blockMask; // 벽/타워 레이어
    [Range(0.6f, 1f)] public float boxScale = 0.90f; // 셀 경계 여유

    [Header("Agent Clearance")]
    public float agentRadius = 0.15f;

    public event Action OnGridChanged;

    bool[,] walkable;
    readonly Collider2D[] buf = new Collider2D[8];

    private void Awake()
    {
        RefreshAllFromPhysics();
    }

    //전체 스캔
    public void RefreshAllFromPhysics()
    {
        if (walkable == null || walkable.GetLength(0) != size.x || walkable.GetLength(1) != size.y)
            walkable = new bool[size.x, size.y];

        Vector2 box = new Vector2(
            Mathf.Max(0.01f, cellSize.x - agentRadius * 2f),
            Mathf.Max(0.01f, cellSize.y - agentRadius * 2f)
            ) * boxScale;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 c = CellCenterWorld(new Vector2Int(x, y));
                int n = Physics2D.OverlapBoxNonAlloc(c, box, 0f, buf, blockMask);
                walkable[x, y] = (n == 0);
            }

        }

        OnGridChanged?.Invoke();
    }

    // 단일 셀 갱신
    public void RefreshCellAtWorld(Vector3 worldPos)
    {
        var c = WorldToCell(worldPos);
        if (!InBounds(c)) return;

        Vector2 box = new Vector2(
            Mathf.Max(0.01f, cellSize.x - agentRadius * 2f),
            Mathf.Max(0.01f, cellSize.y - agentRadius * 2f)
            ) * boxScale;

        int n = Physics2D.OverlapBoxNonAlloc(CellCenterWorld(c), box, 0f, buf, blockMask);
        walkable[c.x, c.y] = (n == 0);
        OnGridChanged?.Invoke();
    }

    // 콜라이더 범위에 걸친 여러 셀 한 번에 갱신
    public void RefreshCellsByBounds(Bounds b)
    {
        float ex = cellSize.x * 0.05f, ey = cellSize.y * 0.05f;
        var min = new Vector3(b.min.x - ex, b.min.y - ey, 0f);
        var max = new Vector3(b.max.x + ex, b.max.y + ey, 0f);

        var cMin = WorldToCell(min);
        var cMax = WorldToCell(max);

        Vector2 box = new Vector2(
            Mathf.Max(0.01f, cellSize.x - agentRadius * 2f),
            Mathf.Max(0.01f, cellSize.y - agentRadius * 2f)
            ) * boxScale;

        for (int x = Mathf.Min(cMin.x, cMax.x); x <= Mathf.Max(cMin.x, cMax.x); x++)
            for (int y = Mathf.Min(cMin.y, cMax.y); y <= Mathf.Max(cMin.y, cMax.y); y++)
            {
                var c = new Vector2Int(x, y);
                if (!InBounds(c)) continue;
                int n = Physics2D.OverlapBoxNonAlloc(CellCenterWorld(c), box, 0f, buf, blockMask);
                walkable[x, y] = (n == 0);
            }

        OnGridChanged?.Invoke();
    }

    // 전체 리패스 신호만 쏘고 싶을 때
    public void ForceRepathAll() => OnGridChanged?.Invoke();



    public bool InBounds(Vector2Int c) => c.x >= 0 && c.y >= 0 && c.x < size.x && c.y < size.y;
    public bool IsWalkable(Vector2Int c) => InBounds(c) && walkable[c.x, c.y];

    public Vector2Int WorldToCell(Vector3 w)
    {
        int cx = Mathf.FloorToInt((w.x - origin.x) / cellSize.x);
        int cy = Mathf.FloorToInt((w.y - origin.y) / cellSize.y);
        return new Vector2Int(cx, cy);
    }

    public Vector3 CellCenterWorld(Vector2Int c) => new Vector3(origin.x + (c.x) * cellSize.x, origin.y + (c.y) * cellSize.y, 0f);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var p = CellCenterWorld(new Vector2Int(x, y));
                Gizmos.DrawWireCube(p, (Vector3)cellSize);
            }
        }
    }
}
