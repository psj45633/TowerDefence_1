using UnityEngine;
using System;

public enum EnemyOutcome { Died, Escaped }

public class Enemy : MonoBehaviour
{
    public static event Action<Enemy, EnemyOutcome> OnEnemyRemoved;

    private ObjectPool pool;
    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    public void SetPool(ObjectPool p) => pool = p;

    public void Init(EnemySO data)
    {
        if(!enemyStats) enemyStats = GetComponent<EnemyStats>();

        enemyStats.OnDied -= OnKilled;

        enemyStats.Init(data.MaxHp);

        enemyStats.OnDied += OnKilled;
    }
    
    public void OnEscaped()
    {
        OnEnemyRemoved?.Invoke(this, EnemyOutcome.Escaped);
        Despawn();
    }

    public void OnKilled()
    {
        OnEnemyRemoved?.Invoke(this, EnemyOutcome.Died);
        Despawn();
    }

    private void Despawn()
    {
        pool.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            OnEscaped();
        }
    }
}
