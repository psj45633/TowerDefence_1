using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEditor.Rendering;
using System.Net;

public class TowerPlacer : MonoBehaviour
{
    public Tilemap buildableMap;
    public Transform towerParent;
    public PathGrid2D grid;

    [Header("Preview Indicator")]
    public Sprite indicatorSprite;
    private float indicatorAlpha = 0.25f;
    private int indicatorSortingOffset = -10;
    private GameObject indicatorObj;
    private SpriteRenderer indicatorRenderer;

    [Header("Block cells")]
    [SerializeField] Vector2 startWorld = new Vector2(-6f, 12f);
    [SerializeField] Vector2 endWorld = new Vector2(6f, -11f);

    private bool buildMode = false;
    public GameObject towerPrefab;
    private GameObject previewObj;
    private HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();

    Vector3Int startCell;
    Vector3Int endCell;

    private readonly List<Vector2Int> tmpCells = new List<Vector2Int>();

    [SerializeField] private GoldManager goldManager;

    private void Awake()
    {
        startCell = buildableMap.WorldToCell((Vector3)startWorld);
        endCell = buildableMap.WorldToCell((Vector3)endWorld);
    }


    public void SetBuildMode(bool active)
    {
        buildMode = active;

        if (active && towerPrefab != null)
        {
            if (previewObj == null)
            {
                previewObj = Instantiate(towerPrefab);
                previewObj.transform.SetParent(towerParent, true);
                foreach (var col in previewObj.GetComponentsInChildren<Collider2D>()) Destroy(col);

                indicatorObj = new GameObject("BuildIndicator");
                indicatorObj.transform.SetParent(previewObj.transform, false);
                indicatorRenderer = indicatorObj.AddComponent<SpriteRenderer>();
                indicatorRenderer.sprite = indicatorSprite;
                indicatorRenderer.color = new Color(0f, 1f, 0f, indicatorAlpha);
                indicatorRenderer.sortingOrder = indicatorSortingOffset;
            }
        }
        else
        {
            CleanupPreview();
        }
    }
    public void SetTowerPrefab(GameObject prefab)
    {
        towerPrefab = prefab;

        if (buildMode)
        {
            SetBuildMode(false);
            SetBuildMode(true);
        }
    }

    void CleanupPreview()
    {
        if (indicatorObj) Destroy(indicatorObj);
        indicatorObj = null; indicatorRenderer = null;

        if (previewObj) Destroy(previewObj);
        previewObj = null;
    }



    private void Update()
    {
        if (!buildMode || towerPrefab == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0;
        Vector3Int cell = buildableMap.WorldToCell(mouseWorld);
        Vector3 cellCenter = buildableMap.GetCellCenterWorld(cell);

        bool isOverBoard = buildableMap.HasTile(cell);

        if (previewObj != null)
            previewObj.SetActive(isOverBoard);

        if (!isOverBoard) return;

        bool onBlockedCell = (cell == startCell) || (cell == endCell);

        // 기존: 타일 점유/스타트/엔드만 보던 로직
        bool canBuild = !occupied.Contains(cell) && !onBlockedCell;

        // 프리뷰 범위가 덮는 셀을 가상 차단으로 가정하고 경로가 남아있는지 미리 검사
        if (grid != null && towerPrefab != null)
        {
            // 1) 프리팹의 콜라이더 bounds 합치기
            Bounds previewBounds;
            var srcCols = towerPrefab.GetComponentsInChildren<Collider2D>();
            if (srcCols != null && srcCols.Length > 0)
            {
                previewBounds = srcCols[0].bounds;
                for (int i = 1; i < srcCols.Length; i++)
                    previewBounds.Encapsulate(srcCols[i].bounds);

                // 프리뷰는 현재 셀 중앙에 둘 것이므로 중심 보정
                previewBounds.center = cellCenter;
            }
            else
            {
                // 콜라이더가 없으면 한 칸 정도로 가정
                previewBounds = new Bounds(cellCenter, new Vector3(0.9f, 0.9f, 0f));
            }

            // 2) 그 bounds가 덮는 셀들을 수집
            tmpCells.Clear();
            grid.GetCellsInBounds(previewBounds, tmpCells);

            // 3) Start/End 셀 (Vector2Int) 준비
            var s = grid.WorldToCell(buildableMap.GetCellCenterWorld(startCell));
            var e = grid.WorldToCell(buildableMap.GetCellCenterWorld(endCell));

            // 4) Start/End 자체를 가상차단에 포함하면 무조건 막힘 처리
            bool touchesSE = tmpCells.Contains(s) || tmpCells.Contains(e);

            // 5) 가상 차단(tmpCells)을 적용했다고 가정하고 경로가 남는지 확인
            bool pathOk = !touchesSE && WouldNotBlockPath(grid, s, e, tmpCells, allowDiagonal: true, cornerCutBlock: true);

            // 최종 판정에 반영
            canBuild = canBuild && pathOk;
        }
        // ─────────────────────────────────────────────────────────────

        if (previewObj != null)
        {
            previewObj.transform.position = cellCenter;

            if (indicatorRenderer != null)
            {
                indicatorRenderer.color = canBuild
                    ? new Color(0f, 1f, 0f, indicatorAlpha)
                    : new Color(1f, 0f, 0f, indicatorAlpha);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (!canBuild)
            {
                Debug.Log("그곳에 지을 수 없음");
                return;
            }

            int cost = towerPrefab.CompareTag("Tower")
                ? towerPrefab.GetComponent<Tower>().towerData.levels[0].cost
                : towerPrefab.GetComponent<Wall>().cost;
            ConsumeGold(cost);

            var inst = Instantiate(towerPrefab, cellCenter, Quaternion.identity, towerParent);
            occupied.Add(cell);

            // 새로 놓은 타워 콜라이더가 덮는 모든 셀을 갱신
            if (grid && inst)
            {
                var cols = inst.GetComponentsInChildren<Collider2D>();
                if (cols != null && cols.Length > 0)
                {
                    var total = cols[0].bounds;
                    for (int i = 1; i < cols.Length; i++) total.Encapsulate(cols[i].bounds);
                    grid.RefreshCellsByBounds(total);  // 다셀 갱신
                }
                else
                {
                    grid.RefreshCellAtWorld(cellCenter); // (콜라이더가 한 칸이면 대안)
                }

                grid.ForceRepathAll(); // 모든 에이전트 즉시 리패스
            }

            CleanupPreview();
            buildMode = false;
        }

    }

    bool WouldNotBlockPath(PathGrid2D g, Vector2Int start, Vector2Int goal,
        List<Vector2Int> extraBlocks, bool allowDiagonal, bool cornerCutBlock)
    {
        var blocked = new HashSet<Vector2Int>(extraBlocks);
        var q = new Queue<Vector2Int>();
        var seen = new HashSet<Vector2Int>();

        // 4방/8방
        Vector2Int[] DIR4 = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        Vector2Int[] DIR8 = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        var dirs = allowDiagonal ? DIR8 : DIR4;

        // 시작/목표가 워크어블인지 + 추가벽에 안 걸리는지
        if (!g.InBounds(start) || !g.InBounds(goal)) return false;
        if (!g.IsWalkable(start) || !g.IsWalkable(goal)) return false;
        if (blocked.Contains(start) || blocked.Contains(goal)) return false;

        q.Enqueue(start);
        seen.Add(start);

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur == goal) return true;

            foreach (var d in dirs)
            {
                var nb = cur + d;
                if (!g.InBounds(nb) || seen.Contains(nb)) continue;
                if (blocked.Contains(nb)) continue;      // 프리뷰가 덮는 셀은 가상 벽
                if (!g.IsWalkable(nb)) continue;         // 원래부터 막힌 셀

                // 대각 코너컷 방지
                if (allowDiagonal && cornerCutBlock && d.x != 0 && d.y != 0)
                {
                    var sideA = new Vector2Int(cur.x + d.x, cur.y);
                    var sideB = new Vector2Int(cur.x, cur.y + d.y);
                    if (!g.IsWalkable(sideA) || !g.IsWalkable(sideB)) continue;
                    if (blocked.Contains(sideA) || blocked.Contains(sideB)) continue;
                }

                seen.Add(nb);
                q.Enqueue(nb);
            }
        }
        return false;
    }

    private void ConsumeGold(int gold)
    {
        if (!goldManager.CanAfford(gold)) return;
        
        goldManager.TrySpend(gold);
    }

    private void NotEnoughGoldTxt()
    {

    }
}
