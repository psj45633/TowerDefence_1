using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField] private Transform target;      // 타겟 Transform(예: Player)
    [SerializeField] private Vector3 targetPosition; // 혹은 Vector3를 계속 쓰면 이 값 사용

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    IEnumerator Start()
    {
        // 한 프레임 대기: NavMeshSurface/씬 초기화 시간 확보
        yield return null;

        // 가장 가까운 NavMesh 지점으로 스냅
        if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
        {
            _agent.Warp(hit.position); // Move/SetDestination 말고 Warp로 즉시 위치 고정
        }
        else
        {
            Debug.LogWarning("[Enemy] 주변에 NavMesh가 없습니다.");
        }
    }

    void Update()
    {
        if (_agent != null && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            if (target != null)
                _agent.SetDestination(target.position);
            else
                _agent.SetDestination(targetPosition);
        }
    }
}
