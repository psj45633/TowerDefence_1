using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    [Header("Buttons")]
    public Button[] buildButtons;
    public Button cancel;

    [Header("Placer")]
    public TowerPlacer placer;

    private int selectedIndex = -1;

    private void Start()
    {
        for(int i = 0; i < buildButtons.Length; i++)
        {
            int index = i;
            buildButtons[i].onClick.AddListener(() => SelectTower(index));
        }

        cancel.onClick.AddListener(CancelBuild);
    }

    void SelectTower(int index)
    {
        selectedIndex = index;
        placer.SetBuildMode(true);
        placer.SetTowerPrefab(buildButtons[index].GetComponent<TowerButton>().prefab);
        Debug.Log($"Tower{index} selected");
    }

    void CancelBuild()
    {
        selectedIndex--;
        placer.SetBuildMode(false);
        Debug.Log("Build canceled");
    }
}
