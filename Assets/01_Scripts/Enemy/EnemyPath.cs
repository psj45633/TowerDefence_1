using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    [Header("Refs")]
    public PathfinderAStar2D pathfinder;

    [Header("Move")]
    public float arriveEps = 0.001f;
    public float repathIntervel = 0.25f;
    public bool repathOnRequest = true;

    Rigidbody2D rb;
    Enemy enemy;

    readonly List<Vector2Int> path = new();
    int idx = -1;
    //float repathTimer = 0f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.None;

        pathfinder = GetComponentInParent<PathfinderAStar2D>();
    }

    private void OnEnable()
    {
        if (repathOnRequest)
            PathfinderAStar2D.RepathAllRequested += Repath;
    }

    private void OnDisable()
    {
        if (repathOnRequest)
            PathfinderAStar2D.RepathAllRequested -= Repath;
    }

    public void Init(PathfinderAStar2D pf)
    {
        pathfinder = pf;

        rb.linearVelocity = Vector2.zero;
        path.Clear();
        idx = -1;
        //repathTimer = 0f;

        Repath();
    }

    //private void Update()
    //{
    //    if (!pathfinder || !pathfinder.goal) return;

    //    if (repathIntervel > 0f)
    //    {
    //        repathTimer += Time.deltaTime;
    //        if (repathTimer >= repathIntervel)
    //        {
    //            repathTimer = 0f;
    //            Repath();
    //        }
    //    }
    //}

    private void FixedUpdate()
    {
        if (!pathfinder || !pathfinder.goal || idx < 0 || idx >= path.Count)
        { rb.linearVelocity = Vector2.zero; return; }

        var nextCell = path[idx];
        var nextCenter = pathfinder.CellCenterWorld(nextCell);

        Vector3 pos = transform.position;
        Vector3 newPos = Vector3.MoveTowards(pos, nextCenter, enemy.def.baseMoveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        float dist = (nextCenter - newPos).sqrMagnitude;

        if (dist <= arriveEps * arriveEps)
        {
            rb.MovePosition(nextCenter);
            idx++;
        }
    }

    public void Repath()
    {
        if (!pathfinder || !pathfinder.goal) return;

        if (pathfinder.TryBuildPathFromWorld(transform.position, path))
        {
            var s = pathfinder.WorldToCell(transform.position);
            idx = (path.Count >= 2 && path[0] == s) ? 1 : 0;
        }
        else
        {
            idx = -1;
            rb.linearVelocity = Vector2.zero;
        }
    }
}