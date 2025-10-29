using TMPro;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public GameObject prefab;

    [SerializeField] TextMeshProUGUI cost;

    private void Start()
    {
        cost = GetComponentInChildren<TextMeshProUGUI>();
        int towerCost = prefab.CompareTag("Tower")
            ? prefab.GetComponent<Tower>().towerData.levels[0].cost
            : prefab.GetComponent<Wall>().cost ;
        cost.text = towerCost.ToString();
    }
}
