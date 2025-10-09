using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    public Tilemap buildableMap;
    public Transform towerParent;

    private bool buildMode = false;
    private GameObject towerPrefab;
    private GameObject previewObj;
    private SpriteRenderer previewRenderer;
    private HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();

    public void SetBuildMode(bool active)
    {
        buildMode = active;

        if (active && towerPrefab != null)
        {
            if (previewObj == null)
            {
                previewObj = Instantiate(towerPrefab);
                previewRenderer = previewObj.GetComponent<SpriteRenderer>();
                if (previewRenderer != null)
                    previewRenderer.color = new Color(0f, 1f, 0f, 0.5f);

            }
        }
        else
        {
            if (previewObj != null)
                Destroy(previewObj);
        }
    }
    public void SetTowerPrefab(GameObject prefab) => towerPrefab = prefab;

    private void Update()
    {
        if (!buildMode || towerPrefab == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int cell = buildableMap.WorldToCell(mouseWorld);
        Vector3 cellCenter = buildableMap.GetCellCenterWorld(cell);

        bool isOverBoard = buildableMap.HasTile(cell);

        if (previewObj != null)
            previewObj.SetActive(isOverBoard);

        if (!isOverBoard) return;


        bool canBuild = buildableMap.HasTile(cell) && !occupied.Contains(cell);

        if (previewObj != null)
        {
            previewObj.transform.position = cellCenter;

            if (previewRenderer != null)
            {
                previewRenderer.color = canBuild ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && canBuild)
        {
            Instantiate(towerPrefab, cellCenter, Quaternion.identity, towerParent);
            occupied.Add(cell);
        }
    }
}
