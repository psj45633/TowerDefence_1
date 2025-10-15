using UnityEngine;

public class ProjectileAttack : MonoBehaviour, ITowerAttack
{
    [SerializeField] private Transform firePoint;

    private Tower owner;

    private float angularSpeed = 3600f;

    private float aimToleranceDeg = 10f;

    private void Awake()
    {
        owner = GetComponentInParent<Tower>();

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
        if (!CanFire(target)) return;

        Debug.Log("Projectile Attack");


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
        Vector2 dir = (t.position - gameObject.transform.position);
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
