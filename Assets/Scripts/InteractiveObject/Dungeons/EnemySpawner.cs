using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public RoomControl room; // 把RoomController拖进去，Spawner会把生成的敌人注册到该房间
    public GameObject enemyPrefab; // 带 Entity 的预制体
    public Transform[] spawnPoints;

    [Header("Wave settings")]
    public bool waveMode = true;
    public List<Wave> waves = new List<Wave>();
    public bool loopWaves = false;

    [Header("Continuous spawn settings")]
    public float spawnRate = 1f; // 每秒生成多少个（spawnRate > 0）
    public int maxConcurrent = 10; // 持续模式的并发上限
    public int totalToSpawn = 0; // <=0 表示无限

    public int currentWaveIndex = 0;
    private int activeCount = 0;
    private int totalSpawned = 0;

    [System.Serializable]
    public class Wave
    {
        public int count = 5;
        public float delayBetweenSpawns = 0.5f;
    }

    private Coroutine runningCoroutine;

    private void Start()
    {
        if (room == null)
        {
            Debug.LogWarning("Spawner: Room not assigned. Trying to find parent RoomControl.");
            room = GetComponentInParent<RoomControl>();
        }

        // 防护：确保 spawnPoints 不被替换为 null 数组误用（保持为 null 表示使用 spawner.transform）
        if (spawnPoints != null && spawnPoints.Length == 0)
            spawnPoints = null;

        if (waveMode)
            runningCoroutine = StartCoroutine(RunWaves());
        else
            runningCoroutine = StartCoroutine(ContinuousSpawn());
    }

    private void OnDisable()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
    }

    #region Wave Mode
    private IEnumerator RunWaves()
    {
        while (true)
        {
            if (currentWaveIndex >= waves.Count)
            {
                if (loopWaves) currentWaveIndex = 0;
                else yield break;
            }

            Wave w = waves[currentWaveIndex];
            for (int i = 0; i < w.count; i++)
            {
                SpawnOne();
                yield return new WaitForSeconds(w.delayBetweenSpawns);
            }

            // 等待房间内当前敌人清空（直接调用 RoomControl 的方法）
            yield return new WaitUntil(() => GetRoomEnemyCount() == 0);

            currentWaveIndex++;
            room.CheckClearCondition();
            yield return null;
        }
    }
    #endregion

    #region Continuous Mode
    private IEnumerator ContinuousSpawn()
    {
        while (totalToSpawn <= 0 || totalSpawned < totalToSpawn)
        {
            if (activeCount < maxConcurrent)
            {
                SpawnOne();
            }
            yield return new WaitForSeconds(1f / Mathf.Max(0.0001f, spawnRate));
        }
    }
    #endregion

    private void SpawnOne()
    {
        Transform spawnPoint = (spawnPoints == null || spawnPoints.Length == 0)
            ? this.transform
            : spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (enemyPrefab == null)
        {
            Debug.LogWarning("Spawner: enemyPrefab is not assigned.");
            return;
        }

        GameObject go = Instantiate(enemyPrefab, spawnPoint.position,Quaternion.identity);
        Entity e = go.GetComponent<Entity>();
        if (e != null)
        {
            // 注册到 room（推荐）
            room?.RegisterEnemy(e);

            // 订阅死亡以维护 spawner 的 activeCount（注意：RoomControl 也订阅自己的逻辑）
            // 为避免重复订阅，先尝试退订
            e.onEnemyDied -= OnEnemyDied;
            e.onEnemyDied += OnEnemyDied;

            activeCount++;
            totalSpawned++;
            totalSpawned = Mathf.Max(totalSpawned, 0);
            Debug.Log($"Spawner: Spawned enemy. activeCount={activeCount}, totalSpawned={totalSpawned}");
        }
        else
        {
            Debug.LogWarning($"Spawner: Spawned object {go.name} has no Entity component.");
        }
    }

    private void OnEnemyDied(Entity e)
    {
        // 防护：有可能已经被退订/销毁
        activeCount = Mathf.Max(0, activeCount - 1);
        if (e != null)
            e.onEnemyDied -= OnEnemyDied;
    }

    private int GetRoomEnemyCount()
    {
        if (room == null)
            return activeCount;

        // 直接调用公开接口（你需要在 RoomControl 中实现 GetEnemyCount()）
        return room.GetEnemyCount();
    }

    public bool IfAllWavesClear()
    {
        if(currentWaveIndex == waves.Count )
        {
            return true;
        }
        else return false;
    }
}
