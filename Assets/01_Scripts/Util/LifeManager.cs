using System;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private int maxLife = 20;
    public int MaxLife => maxLife;
    public int CurLife { get; private set; }

    public static event Action<int, int> OnLifeChaged;

    private void OnEnable()
    {
        Enemy.OnEnemyRemoved += HandleEnemyRemoved;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyRemoved -= HandleEnemyRemoved;
    }


    private void Start()
    {
        ResetLife();
    }

    void HandleEnemyRemoved(Enemy enemy, EnemyOutcome outcome)
    {
        if (outcome == EnemyOutcome.Escaped)
        {
            LoseLife(1);
        }
    }

    public void ResetLife()
    {
        CurLife = maxLife;
        OnLifeChaged?.Invoke(CurLife, maxLife);
    }

    public void LoseLife(int amount = 1)
    {
        if (amount <= 0) return;

        CurLife -= amount;
        if (CurLife < 0) CurLife = 0;

        OnLifeChaged?.Invoke(CurLife, maxLife);

        if (CurLife == 0)
        {
            // GameManager.Instance.GameOver();
        }
    }

    public void AddLife(int amount = 1)
    {
        if (amount <= 0) return;

        CurLife = Mathf.Min(maxLife, CurLife + amount);

        OnLifeChaged?.Invoke(CurLife, maxLife);
    }

    public int GetMaxLife() => maxLife;

}
