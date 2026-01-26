using System.Collections.Generic;
using UnityEngine;

public class PathPreviewLine : MonoBehaviour
{
    [Header("Refs")]
    public PathfinderAStar2D pathfinder;
    public Vector3 start;

    private readonly HashSet<Vector2Int> pathCells = new();
    public bool IsPathCell(Vector2Int cell) => pathCells.Contains(cell);

    private LineRenderer line;
    private readonly List<Vector2Int> path = new();

    bool built;

    public int LineCount => line ? line.positionCount : 0;
    public Vector3 GetLinePos(int i) => line.GetPosition(i);

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        
    }

    private void Start()
    {
        Rebuild();
    }

    private void OnEnable()
    {
        PathfinderAStar2D.RepathAllRequested += Rebuild;
        Rebuild();
    }

    private void OnDisable()
    {
        PathfinderAStar2D.RepathAllRequested -= Rebuild;
    }

    private void Update()
    {
        if (built) return;
        if (!pathfinder || !pathfinder.grid || !pathfinder.goal) return;

        if (pathfinder.TryBuildPathFromWorld(start, path))
        {
            line.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
                line.SetPosition(i, pathfinder.CellCenterWorld(path[i]));
            built = true;
        }
    }

    public void Rebuild()
    {
        if (!pathfinder || !pathfinder.goal || !pathfinder.grid) { Clear(); return; }

        if (!pathfinder.TryBuildPathFromWorld(start, path))
        {
            Clear();
            return;
        }

        pathCells.Clear();
        for (int i = 0; i < path.Count; i++)
        {
            pathCells.Add(path[i]);
        }

        line.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            var p = pathfinder.CellCenterWorld(path[i]);
            line.SetPosition(i, p);
        }
    }

    //    static readonly Vector2Int[] DIR4 =
    //{
    //    new(1,0), new(-1,0), new(0,1), new(0,-1),
    //    new(1,1), new(1,-1), new(-1,1), new(-1,-1)
    //};

    //    public void Rebuild()
    //    {
    //        if (!pathfinder || !pathfinder.grid || !pathfinder.goal)
    //        {
    //            Clear();
    //            return;
    //        }

    //        if (!pathfinder.TryBuildPathFromWorld(start, path)) // Raw = 압축 없는 full path 권장
    //        {
    //            Clear();
    //            return;
    //        }

    //        pathCells.Clear();

    //        for (int i = 0; i < path.Count; i++)
    //        {
    //            var c = path[i];

    //            // 경로 셀
    //            if (pathfinder.grid.InBounds(c))
    //                pathCells.Add(c);

    //            // 주변 1칸(대각 포함)
    //            for (int k = 0; k < DIR4.Length; k++)
    //            {
    //                var nb = c + DIR4[k];
    //                if (pathfinder.grid.InBounds(nb))
    //                    pathCells.Add(nb);
    //            }
    //        }

    //        // 라인 렌더러는 보기용으로만 (압축된 걸 써도 됨)
    //        line.positionCount = path.Count;
    //        for (int i = 0; i < path.Count; i++)
    //            line.SetPosition(i, pathfinder.CellCenterWorld(path[i]));
    //    }

    private void Clear()
    {
        if (!line) return;
        line.positionCount = 0;
    }
}
