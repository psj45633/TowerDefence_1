using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public event Action OnDied;
    public int curHP;

    public void Init(int maxHp)
    {
        curHP = maxHp;
    }

    public void TakeDamage(int dmg)
    {
        curHP -= dmg;
        if(curHP <= 0)
        {
            OnDied?.Invoke();
        }
    }
}
