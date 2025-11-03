using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool enemyPool;
    public EnemySO[] stageEnemies;
    [SerializeField] private Vector3 spawnPoint = Vector3.zero;

    public PathGrid2D grid;
    public Transform goal;

    [Header("Wave Setting")]
    [SerializeField] private int spawnCount = 30;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int currentStage = 0;

    public int spawnIndex = 1;

    private float timer;
    private int spawnedEnemy;
    public bool waveActive = false;
    

    private void Awake()
    {
        if (!enemyPool) enemyPool = GetComponent<ObjectPool>();

        stageEnemies = Resources.LoadAll<EnemySO>("Enemies");

        if (stageEnemies == null || stageEnemies.Length == 0)
        {
            Debug.LogError("stageEnemies가 비어있음.");
        }
    }

    private void Update()
    {
        if (!waveActive) return;
        if (spawnedEnemy >= spawnCount)
        {
            waveActive = false;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }

    }

    private void SpawnEnemy()
    {
        int waveNumber = currentStage;

        // 그 웨이브 번호랑 같은 SO 찾기
        var data = stageEnemies.FirstOrDefault(e => e.stage == waveNumber);
        if (data == null)
        {
            Debug.LogWarning($"stageIndex == {waveNumber} 인 EnemySO를 못 찾음");
            return;
        }

        var go = enemyPool.GetFromPool();
        var cell = grid.WorldToCell(spawnPoint); // 너 스폰포인트 쓰면 거기로
        go.transform.position = grid.CellCenterWorld(cell);

        var enemy = go.GetComponent<Enemy>();
        enemy.SetPool(enemyPool);
        enemy.Init(data);

        var agent = go.GetComponent<TileAgentAStar2D>();
        if (agent && grid && goal)
            agent.Init(grid, goal);

        spawnedEnemy++;
    }

    public void StartWave()
    {
        spawnedEnemy = 0;
        timer = 0f;
        currentStage++;
        Debug.Log(currentStage);
        waveActive = true;

        TileAgentAStar2D.RequestRepathAll();
    }
}
