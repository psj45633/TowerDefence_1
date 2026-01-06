using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject inGameMenu;

    void Start()
    {
        buildMenu.SetActive(false);
        upgradeMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }

    public void ActiveSelfBuildMenu()
    {
        buildMenu.SetActive(!buildMenu.activeSelf);
        inGameMenu.SetActive(!inGameMenu.activeSelf);
    }

    public void ActiveSelfUpgradeMenu()
    {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        inGameMenu.SetActive(!inGameMenu.activeSelf);
    }

}
