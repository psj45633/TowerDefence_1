using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    [Header("Buttons")]
    public Button[] buildButtons;
    public Button cancel;

    [Header("Placer")]
    public TowerPlacer placer;

    [SerializeField] private GoldManager goldManager;
    [SerializeField] private EnemySpawner spawner;

    [SerializeField] private TxtHandler txtHandler;

    private int selectedIndex = -1;

    private void Start()
    {
        for (int i = 0; i < buildButtons.Length; i++)
        {
            int index = i;
            buildButtons[i].onClick.AddListener(() => SelectTower(index));
        }

        cancel.onClick.AddListener(CancelBuild);
    }

    void SelectTower(int index)
    {
        //if (spawner.waveActive)
        //{
        //    cantBuildTxt.gameObject.SetActive(true);
        //    return;
        //}

        Enemy[] enemies = spawner.GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy || spawner.waveActive)
            {
                txtHandler.ActiveTxt(2);
                return;
            }
        }

        var tower = buildButtons[index].GetComponent<TowerButton>().prefab;
        int towerCost = tower.CompareTag("Tower")
            ? tower.GetComponent<Tower>().towerData.levels[0].cost
            : tower.GetComponent<Wall>().cost;
        if (towerCost > goldManager.Gold)
        {
            txtHandler.ActiveTxt(0);
            return;
        }
        selectedIndex = index;
        placer.SetBuildMode(true);
        placer.SetTowerPrefab(buildButtons[index].GetComponent<TowerButton>().prefab);
        Debug.Log($"Tower{index} selected");
    }

    void CancelBuild()
    {
        selectedIndex = -1;
        placer.SetBuildMode(false);
        Debug.Log("Build canceled");
    }
}
