using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public Tilemap buildableMap;
    public Transform towerParent;

    [Header("Preview Indicator")]
    public Sprite indicatorSprite;
    private float indicatorAlpha = 0.25f;
    private int indicatorSortingOffset = -10;
    private GameObject indicatorObj;
    private SpriteRenderer indicatorRenderer;


    private bool buildMode = false;
    private GameObject towerPrefab;
    private GameObject previewObj;
    private HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();

    public void SetBuildMode(bool active)
    {
        buildMode = active;

        if (active && towerPrefab != null)
        {
            if (previewObj == null)
            {
                previewObj = Instantiate(towerPrefab);
                foreach(var col in previewObj.GetComponentsInChildren<Collider2D>()) Destroy(col);

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


        bool canBuild = !occupied.Contains(cell);

        if (previewObj != null)
        {
            previewObj.transform.position = cellCenter;

            if (indicatorRenderer != null)
            {
                indicatorRenderer.color = canBuild ? new Color(0f, 1f, 0f, indicatorAlpha) : new Color(1f, 0f, 0f, indicatorAlpha);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && canBuild)
        {
            Instantiate(towerPrefab, cellCenter, Quaternion.identity, towerParent);
            occupied.Add(cell);
            CleanupPreview();
            buildMode = false;
        }
    }
}
