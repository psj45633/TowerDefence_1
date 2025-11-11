using TMPro;
using UnityEngine;

public class LifeUI : MonoBehaviour
{
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private TMP_Text lifeText;

    private void OnEnable()
    {
        LifeManager.OnLifeChaged += UpdateUI;
    }
    private void OnDisable()
    {
        LifeManager.OnLifeChaged -= UpdateUI;
    }
    private void Start()
    {
        if (lifeManager) UpdateUI(lifeManager.CurLife, lifeManager.MaxLife);
    }
    private void UpdateUI(int cur, int max)
    {
        if (lifeText) lifeText.text = $"{cur} / {max}";
    }
}
