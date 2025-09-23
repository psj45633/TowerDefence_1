using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public event Action OnDied;
    public int hp;

    public void Init(int maxHp)
    {
        hp = maxHp;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            OnDied?.Invoke();
        }
    }
}
