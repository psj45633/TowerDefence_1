using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TowerMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject menuObj;
    [SerializeField] private Image curInfoImage;
    public GameObject rangeMarker;

    public void TowerMenu()
    {
        var curRange = GetComponentInParent<Tower>().curRange;
        rangeMarker.transform.localScale = Vector3.one * curRange * 2;

        var active = menuObj.gameObject.activeInHierarchy;
        menuObj.gameObject.SetActive(!active);
        curInfoImage.gameObject.SetActive(false);
    }
}
