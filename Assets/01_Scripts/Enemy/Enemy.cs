using UnityEngine;
using System;

public enum EnemyOutcome { Killed, Escaped }

public class Enemy : MonoBehaviour
{
    public static event Action<Enemy, EnemyOutcome> OnFinished;

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
        Debug.Log(enemyStats.hp);

        enemyStats.OnDied += OnKilled;
    }
    
    public void OnEscaped()
    {
        OnFinished?.Invoke(this, EnemyOutcome.Escaped);
        Despawn();
    }

    public void OnKilled()
    {
        OnFinished?.Invoke(this, EnemyOutcome.Killed);
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
