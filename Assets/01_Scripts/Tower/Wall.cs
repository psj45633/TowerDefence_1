using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Wall : MonoBehaviour
{
    public int cost;

    private GoldManager goldManager;

    [SerializeField] private GameObject menuObj;

    
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

        var placer = FindFirstObjectByType<TowerPlacer>();
        placer.occupied.Remove(placer.cell);
    }
}
