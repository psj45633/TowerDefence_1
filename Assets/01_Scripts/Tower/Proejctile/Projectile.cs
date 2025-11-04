using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Tower owner;
    private ObjectPool pool;
    public Enemy target;
    public float moveSpeed = 20f;
    private float spriteForwardOffset = 0f;

    private void Start()
    {
        owner = GetComponentInParent<Tower>();
        pool = owner.GetComponentInChildren<ObjectPool>();
    }

    public void Init(Enemy t)
    {
        target = t;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            pool.ReturnToPool(gameObject);
        }
    }

    private void Update()
    {
        // 현재 위치 → 타겟 방향
        Vector3 pos = transform.position;
        Vector3 targetPos = target.transform.position;
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
}
