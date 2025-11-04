using UnityEngine;

public class ProjectileAttack : MonoBehaviour, ITowerAttack
{
    [SerializeField] private Transform firePoint;

    private Tower owner;

    private ObjectPool projectilePool;

    [Header("Aim")]
    private float angularSpeed = 3600f;
    private float aimToleranceDeg = 10f;

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float spriteForwardOffset = 0f;

    private void Awake()
    {
        owner = GetComponentInParent<Tower>();
        projectilePool = GetComponent<ObjectPool>();
    }

    public void Init(Tower o) => owner = o;

    public void Apply(TowerInfoSO data)
    {
        var lv = data.levels[owner.currentLevelIndex];
    }

    public bool CanFire(Enemy target)
    {
        return target && IsAimedAt(target.transform);
    }

    public void Attack(Enemy target)
    {
        if (!CanFire(target) || !firePoint || !projectilePool) return;

        // 방향 / 각도 계산
        Vector3 pos = firePoint.position;
        Vector2 dir = (target.transform.position - pos).normalized;
        float angz = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + spriteForwardOffset;
        Quaternion rot = Quaternion.AngleAxis(angz, Vector3.forward);

        // 풀에서 투사체 꺼내기
        GameObject projGO = projectilePool.GetFromPool();

        // 위치 / 회전
        projGO.transform.SetPositionAndRotation(pos, rot);

        // 타겟 세팅
        Projectile proj = projGO.GetComponent<Projectile>();
        proj.Init(target);

        // 이동 방식 세팅
        if (proj.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = -transform.up;
            rb.angularVelocity = 0f;
        }
    }

    private void Update()
    {
        var target = owner.targeter.currentTarget;

        if (target != null)
        {
            AimToTarget(target.transform, angularSpeed);
        }
    }

    private void AimToTarget(Transform t, float angSpd)
    {
        if (!t) return;
        Vector2 dir = (t.position - transform.position);
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        float z = Mathf.MoveTowardsAngle(gameObject.transform.eulerAngles.z, ang, angSpd * Time.deltaTime);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, z);
    }

    private bool IsAimedAt(Transform t)
    {
        if (!t) return false;
        Vector2 dir = (t.position - transform.position);
        float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        float delta = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetZ));
        return delta <= aimToleranceDeg;
    }
}
