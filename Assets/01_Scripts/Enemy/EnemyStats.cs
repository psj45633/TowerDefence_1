using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public event Action OnDied;
    public int curHP;
    public int MaxHP;
    public float hpPercentage;
    Enemy enemy;

    [SerializeField] private Image hpBar;
    private bool onHpBar = false;
    private float hpBarTimer;

    public void Init(int maxHp, float speed)
    {
        enemy = GetComponent<Enemy>();
        curHP = maxHp;
        MaxHP = maxHp;
        hpBar.gameObject.SetActive(false);
    }

    public void TakeDamage(int dmg)
    {
        onHpBar = true;
        hpBarTimer = 2f;
        curHP = Mathf.Max(0, curHP - dmg);
        UpdateHPBar();
        if (curHP <= 0)
        {
            OnDied?.Invoke();
        }
    }

    private void UpdateHPBar()
    {
        float denom = Mathf.Max(1, MaxHP);
        hpPercentage = Mathf.Clamp01((float)curHP / denom);
        if (hpBar)
        {
            hpBar.fillAmount = hpPercentage;
            if(hpPercentage >= 0.5f)
            {
                hpBar.color = new Color((1-hpPercentage)*2f, 1f,0f);
            }
            else
            {
                hpBar.color = new Color(1f,hpPercentage*2f , 0f);
            }
            
        }
    }

    private void Update()
    {
        if (onHpBar)
        {
            OnHpBar(true);
            hpBarTimer -= Time.deltaTime;
            if (hpBarTimer <= 0)
            {
                OnHpBar(false);
            }
        }

        if (!onHpBar)
        {
            OnHpBar(false);
        }
    }

    private void OnHpBar(bool on)
    {
        hpBar.gameObject.SetActive(on);
    }




}
