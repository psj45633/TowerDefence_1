using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    [SerializeField] private int gold = 0;
    public int Gold => gold;

    public event Action<int> OnGoldChanged;

    private void OnEnable()
    {
        Enemy.OnEnemyRemoved += HandleEnemyRemoved;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyRemoved -= HandleEnemyRemoved;
    }

    private void HandleEnemyRemoved(Enemy enemy, EnemyOutcome outcome)
    {
        if (outcome != EnemyOutcome.Died) return;

        int reward = (enemy && enemy.def != null) ? enemy.def.rewardGold : 0;
        if (reward <= 0) return;

        gold += reward;
        OnGoldChanged?.Invoke(gold);
    }

    public void Add(int amount)
    {
        if (amount == 0) return;
        int next = gold + amount;
        gold = Mathf.Max(0, Mathf.Min(int.MaxValue, next));
        OnGoldChanged?.Invoke(gold);
    }

    public bool CanAfford(int cost)
    {
        if (cost <= 0) return true;
        return gold >= cost;
    }

    public bool TrySpend(int cost)
    {
        if (cost <= 0) return true;
        if (gold < cost) return false;

        gold -= cost;
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    public void Set(int value)
    {
        gold = Mathf.Max(0,value);
        OnGoldChanged?.Invoke(gold);
    }
}
