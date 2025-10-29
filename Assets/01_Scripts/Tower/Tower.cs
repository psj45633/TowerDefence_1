using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerInfoSO towerData;
    public int currentLevelIndex = 0;

    private float fireRate;

    ITowerAttack attack;
    public TowerTargeter targeter;

    private int wallLayer;

    void Awake()
    {
        attack = GetComponentInChildren<ITowerAttack>();
        attack.Init(this);
        attack.Apply(towerData);
        targeter = GetComponentInChildren<TowerTargeter>();
        fireRate = towerData.levels[currentLevelIndex].fireRate;

        ApplyLevel(0);
    }


    public bool IsMaxLevel => currentLevelIndex >= towerData.levels.Length - 1;

    public void Upgrade()
    {
        if (IsMaxLevel) return;
        ApplyLevel(currentLevelIndex + 1);
    }

    private void ApplyLevel(int levelIndex)
    {
        currentLevelIndex = Mathf.Clamp(levelIndex, 0, towerData.levels.Length - 1);
    }


    float timer;
    void Update()
    {
        var target = targeter.currentTarget;

        timer += Time.deltaTime;
        //if (target != null && timer > fireCooldown && attack.CanFire(target))
        if (target != null && timer > fireRate)
        {
            attack.Attack(target);
            timer = 0;
        }

    }






}
