using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PulseAttack : MonoBehaviour, ITowerAttack
{
    private Tower owner;
    private TowerTargeter targeter;
    private EffectController effect;

    private void Start()
    {
        effect = GetComponent<EffectController>();
    }

    public void Init(Tower o)
    {
        owner = o;

        // 타워 구조에 맞게: 타워 자식에 TowerTargeter가 있다는 가정
        targeter = owner.GetComponentInChildren<TowerTargeter>();
        if (!targeter)
            targeter = GetComponentInChildren<TowerTargeter>();
    }

    public void Apply(TowerInfoSO data)
    {
        var lv = data.levels[owner.currentLevelIndex];
    }

    public bool CanFire(Enemy target)
    {
        return target;
    }

    public void Attack(Enemy target)
    {
        if (!CanFire(target)) return;

        effect.Play();

        int towerDamage = owner.curDamage;

        int n = targeter.ScanInRange(out var buf);

        for (int i = 0; i < n; i++)
        {
            var e = buf[i];
            buf[i] = null; // 다음 스캔 대비 정리(중요)

            if (!e || !e.gameObject.activeInHierarchy) continue;

            e.GetComponent<EnemyStats>().TakeDamage(towerDamage);
        }

        effect.StopEffect();
    }

}
