using System.Collections.Generic;
using UnityEngine;

public class TowerTargeter : MonoBehaviour
{
    [Header("Detect")]
    public LayerMask enemyMask;
    public float retargetInterval = 0.15f;
    [SerializeField] private int maxHits = 30;

    [Header("Result (read-only")]
    public Enemy currentTarget;

    private Collider2D[] hits;
    private ContactFilter2D contactFilter;
    private Enemy[] enemyBuf;

    private float timer;

    public Tower tower;


    private void Awake()
    {
        tower = GetComponentInParent<Tower>();
        hits = new Collider2D[maxHits];
        enemyBuf = new Enemy[maxHits];

        contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = enemyMask;
        contactFilter.useTriggers = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < retargetInterval) return;
        timer = 0f;

        currentTarget = ScanAndPickNearest();
    }

    Enemy ScanAndPickNearest()
    {
        float range = tower.towerData.levels[tower.currentLevelIndex].range;
        int count = Physics2D.OverlapCircle(transform.position, range, contactFilter, hits);

        int enemyCount = 0;
        for(int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col) continue;
            
            if(col.TryGetComponent(out Enemy e) && e)
            {
                if(!e.gameObject.activeInHierarchy) continue;
                enemyBuf[enemyCount++] = e;
            }
        }

        if (enemyCount == 0) return null;

        Enemy nearest = null;
        float nearestDist2 = float.PositiveInfinity;
        Vector3 p = transform.position;

        for(int i = 0; i<enemyCount; i++)
        {
            var e = enemyBuf[i];
            if(!e) continue;

            float d2 = (e.transform.position - p).sqrMagnitude;
            if(d2 < nearestDist2)
            {
                nearestDist2 = d2;
                nearest = e;
            }
            //다음 스캔을 위해 버퍼 슬로 ㅅ정리
            enemyBuf[i] = null;
        }

        return nearest;
    }




}
