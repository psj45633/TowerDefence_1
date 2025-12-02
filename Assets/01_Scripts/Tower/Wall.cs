using UnityEngine;

public class Wall : MonoBehaviour
{
    public int cost;

    private GoldManager goldManager;

    [SerializeField] private GameObject menuObj;
    TowerPlacer ownerPlacer;
    Vector3Int placedCell { get; set; }


    void Awake()
    {
        var gm = Object.FindAnyObjectByType<GoldManager>();
        goldManager = gm;
    }

    public void TowerMenu()
    {
        var active = menuObj.gameObject.activeInHierarchy;
        menuObj.gameObject.SetActive(!active);
    }

    public void Sell()
    {
        goldManager.Add(cost);
        Destroy(gameObject);
        if (ownerPlacer != null)
        {
            ownerPlacer.FreeCell(placedCell, GetComponentsInChildren<Collider2D>());
        }
        var placer = FindFirstObjectByType<TowerPlacer>();
        placer.occupied.Remove(placer.cell);
    }
}
