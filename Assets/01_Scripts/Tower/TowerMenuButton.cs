using UnityEngine;
using UnityEngine.UI;

public class TowerMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject menuObj;
    [SerializeField] private Image curInfoImage;

    public void TowerMenu()
    {
        var active = menuObj.gameObject.activeInHierarchy;
        menuObj.gameObject.SetActive(!active);
        curInfoImage.gameObject.SetActive(false);
    }
}
