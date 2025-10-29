using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private GoldManager goldManager;
    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        if (goldManager) goldManager.OnGoldChanged += UpdateUI;
    }
    private void OnDisable()
    {
        if (goldManager) goldManager.OnGoldChanged -= UpdateUI;
    }
    private void Start()
    {
        if (goldManager) UpdateUI(goldManager.Gold);
    }
    private void UpdateUI(int value)
    {
        if (goldText) goldText.text = value.ToString();
    }
}
