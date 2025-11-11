using UnityEngine;
using System;
using UnityEngine.AI;

public enum EnemyOutcome { Died, Escaped }

public class Enemy : MonoBehaviour
{
    public static event Action<Enemy, EnemyOutcome> OnEnemyRemoved;

    private ObjectPool pool;
    private EnemyStats enemyStats;
    public EnemySO def { get; set; }

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    public void SetPool(ObjectPool p) => pool = p;

    public void Init(EnemySO data)
    {
        def = data;
        var sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = data.tintColor;
        sr.sprite = data.sprite;
        if (!enemyStats) enemyStats = GetComponent<EnemyStats>();

        enemyStats.OnDied -= OnKilled;

        enemyStats.Init(data.MaxHp, data.baseMoveSpeed);

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
        else if (other.CompareTag("Attack"))
        {
            Tower tower = other.GetComponentInParent<Tower>();
            
            int towerLv = tower.currentLevelIndex;
            int towerDamage = tower.towerData.levels[towerLv].damage;
            enemyStats.TakeDamage(towerDamage);

        }
    }
}
