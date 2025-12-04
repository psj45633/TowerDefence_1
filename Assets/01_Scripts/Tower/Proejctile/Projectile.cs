using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Tower owner;
    private ObjectPool pool;
    public Enemy target;
    public float moveSpeed = 20f;
    private float spriteForwardOffset = 0f;
    Vector3 targetPosition;

    [Header("Splash")]
    [SerializeField] private bool isSplash;
    [SerializeField] private float splashRadius = 0.5f;
    [SerializeField, Range(0f, 1f)] private float splashRate = 1f;
    [SerializeField] private LayerMask enemyMask;

    private void Start()
    {
        owner = GetComponentInParent<Tower>();
        pool = owner.GetComponentInChildren<ObjectPool>();
    }

    public void Init(Enemy t)
    {
        target = t;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        int towerDamage = owner.curDamage;

        if (col.CompareTag("Enemy"))
        {

            var hitEnemy = col.GetComponent<EnemyStats>();
            DestroyObj();
            if (isSplash)
            {
                DoSplashDamage(hitEnemy, towerDamage);
            }
            else
            {
                hitEnemy.TakeDamage(towerDamage);
            }
        }
        else if (col.CompareTag("Wall"))
        {
            DestroyObj();
        }
    }

    private void DoSplashDamage(EnemyStats mainEnemy, int towerDamage)
    {
        Vector2 center = transform.position;

        // 범위 안 Enemy 전부 찾기
        var hits = Physics2D.OverlapCircleAll(center, splashRadius, enemyMask);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyStats>();
            if (enemy == null) continue;

            if (enemy == mainEnemy)
            {
                // 대상 풀뎀
                enemy.TakeDamage(towerDamage);
            }
            else
            {
                // 주변 스플 비율
                int splashDamage = Mathf.RoundToInt(towerDamage * splashRate);
                enemy.TakeDamage(splashDamage);
            }
        }
    }

    private void Update()
    {
        if (!target.gameObject.activeInHierarchy)
        {
            if (transform.position == targetPosition)
                DestroyObj();
        }

        SearchTarget();
    }

    private void SearchTarget()
    {
        // 현재 위치 → 타겟 방향
        Vector3 pos = transform.position;
        Vector3 targetPos = target.transform.position;
        targetPosition = targetPos;
        Vector3 toTarget = (targetPos - pos);

        // 1) 앞으로 이동
        // 타겟 위치까지 바로 텔레포트 말고, 일정 속도로 가까워지게
        Vector3 nextPos = Vector3.MoveTowards(pos, targetPos, moveSpeed * Time.deltaTime);
        transform.position = nextPos;

        // 2) 타겟을 향해 회전
        Vector2 dir = toTarget.normalized;
        float angz = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + spriteForwardOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, angz);
    }

    private void DestroyObj()
    {
        pool.ReturnToPool(gameObject);
    }
}
