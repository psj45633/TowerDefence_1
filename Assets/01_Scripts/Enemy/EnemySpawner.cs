using UnityEngine;
using System.Collections.Generic;

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
    private bool waveActive = false;
    

    //private float lastSpawnTime;

    private void Awake()
    {
        if (!enemyPool) enemyPool = GetComponent<ObjectPool>();

        if (stageEnemies == null || stageEnemies.Length == 0)
        {
            Debug.LogError("stageEnemies가 비어있음.");
        }

        //currentStage = Mathf.Clamp(currentStage + 1, 0, stageEnemies.Length - 1);
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
        if (stageEnemies == null || stageEnemies.Length == 0) return;
        if (currentStage < 0 || currentStage > stageEnemies.Length) { Debug.Log("다음스테이지 없음)"); return; }

        var data = stageEnemies[currentStage-1];
        if (data == null) { Debug.LogError($"stageEnemies[{currentStage}]가 null"); }

        var go = enemyPool.GetFromPool();
        var cell = grid.WorldToCell(spawnPoint);
        go.transform.position = grid.CellCenterWorld(cell);

        var enemy = go.GetComponent<Enemy>();

        enemy.SetPool(enemyPool);
        enemy.Init(data);

        var agent = go.GetComponent<TileAgentAStar2D>();
        if (!agent) return;
        if (!grid || !goal) return;

        agent.Init(grid, goal);

        spawnedEnemy++;
        spawnIndex++;

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
