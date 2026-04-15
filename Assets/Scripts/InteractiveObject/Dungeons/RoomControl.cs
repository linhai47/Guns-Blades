using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简化版 RoomControl：
/// - 不再依赖位置判断（不使用 Entity.onEnemySpawnedStatic）
/// - 支持在 Inspector 里预放敌人（enemies）或激活时 spawnPrefabs 生成
/// - 每当敌人死亡（e.onEnemyDied）就从列表移除；当列表为空时打开 gate
/// - 支持玩家进入触发激活或手动 ActivateRoom()
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RoomControl : MonoBehaviour
{
    [Header("Door / Gate")]
    public RoomGate gate; // 指向门的脚本或GameObject（需在Inspector拖拽）

    [Header("Activation")]
    [Tooltip("玩家进入触发房间激活（需要 RoomCollider 为 Is Trigger 且玩家 Tag 为 Player）")]
    public bool PlayerActivated = true;
    [Tooltip("场景开始时是否关闭门（激活时会再次尝试关闭）")]
    public bool closeAtStart = true;

    [Header("Spawn on activate")]
    [Tooltip("激活房间时是否生成 enemyPrefabs（若 false，使用 Inspector 中的 enemies 列表）")]
    public bool spawnOnActivate = false;
    [Tooltip("会在激活时依次生成这些预制体（若 spawnPoints 为空则在 RoomControl 的位置生成）")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    [Tooltip("生成点（可空，空则使用 RoomControl 的 transform）")]
    public Transform[] spawnPoints;

    [Header("Initial enemies (optional)")]
    [Tooltip("如果你已在场景放置敌人，可将它们拖到这里；激活时这些敌人会被注册到房间中")]
    public List<Entity> enemies = new List<Entity>();

    public EnemySpawner[] spawnersOfRoom;

    public DoorCameraBoundary CameraBoundary;

    private bool activated = false;

    private void Awake()
    {
        if (gate == null)
            Debug.LogWarning("RoomControl: Gate not assigned.");

        if (closeAtStart && gate != null)
            gate.Close();
    }

    private void Start()
    {
        // 如果不需要玩家进入就自动激活（并注册已有敌人或生成）
        if (!PlayerActivated)
            ActivateRoom();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PlayerActivated) return;
        if (activated) return;
        if (collision.CompareTag("Player"))
            ActivateRoom();
    }

    /// <summary>
    /// 激活房间：关闭门（如配置），注册 Inspector 中的敌人，或根据预制体生成敌人
    /// </summary>
    public void ActivateRoom()
    {
        if (activated) return;
        activated = true;

        if (closeAtStart && gate != null) gate.Close();
       
        // 注册 Inspector 里已有的敌人（订阅其死亡事件）
        for (int i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (e != null)
                RegisterEnemy(e);
        }

        // 根据配置在激活时生成预制体
        if (spawnOnActivate && enemyPrefabs != null && enemyPrefabs.Count > 0)
        {
            SpawnPrefabs();
            Debug.Log("SpawnPrefabs");
        }

        // 检查是否一开始就清空（没有敌人）
        CheckClearCondition();
        Debug.Log($"RoomControl: Activated. Current enemy count: {enemies.Count}");
    }

    /// <summary>
    /// 将一个已经存在的敌人注册到房间（会订阅其 onEnemyDied 事件）
    /// 如果该敌人已在列表中则不会重复添加（会先做退订保护再订阅）
    /// </summary>
    public void RegisterEnemy(Entity e)
    {
        if (e == null) return;

        if (!enemies.Contains(e))
            enemies.Add(e);

        // 先退订以防重复
        e.onEnemyDied -= OnEnemyDied;
        e.onEnemyDied += OnEnemyDied;

        // 如果房间还未激活且不需要玩家触发，则激活
        if (!activated && !PlayerActivated)
            ActivateRoom();

        Debug.Log($"RoomControl: Registered enemy {e.name}. Count = {enemies.Count}");
    }

    /// <summary>
    /// 当某敌人死亡时回调（由敌人触发 onEnemyDied）
    /// </summary>
    private void OnEnemyDied(Entity e)
    {
        if (e != null)
        {
            e.onEnemyDied -= OnEnemyDied;
            enemies.Remove(e);
            //Debug.Log($"RoomControl: Enemy died {e.name}. Remaining = {enemies.Count}");
        }
        else
        {
            // 防御性处理：清理 null 条目
            enemies.RemoveAll(x => x == null);
            Debug.Log($"RoomControl: Enemy died (null). Cleaned list. Remaining = {enemies.Count}");
        }

        CheckClearCondition();
    }

    /// <summary>
    /// 外部手动移除某个敌人（例如对象被回收或传送走）
    /// </summary>
    public void RemoveEnemy(Entity e)
    {
        if (e == null) return;
        if (enemies.Contains(e))
        {
            e.onEnemyDied -= OnEnemyDied;
            enemies.Remove(e);
            Debug.Log($"RoomControl: Manually removed {e.name}. Remaining = {enemies.Count}");
            CheckClearCondition();
        }
    }

    /// <summary>
    /// 生成 enemyPrefabs 并注册（在 ActivateRoom 时调用）
    /// </summary>
    private void SpawnPrefabs()
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            var prefab = enemyPrefabs[i];
            if (prefab == null) continue;

            Vector3 spawnPos = transform.position;
            if (spawnPoints != null && spawnPoints.Length > 0)
                spawnPos = spawnPoints[i % spawnPoints.Length].position;

            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            var ent = go.GetComponent<Entity>();
            if (ent != null)
            {
                RegisterEnemy(ent);
            }
            else
            {
                Debug.LogWarning($"RoomControl: Spawned prefab {prefab.name} has no Entity component.");
            }
        }
    }

    /// <summary>
    /// 房间清空检测：若敌人列表为空则打开门
    /// </summary>
    public void CheckClearCondition()
    {
        // 去掉所有 null 引用，防止死对象占位
        enemies.RemoveAll(x => x == null);
        bool isAllClear = true;
        foreach (var spawners in spawnersOfRoom)
        {
            isAllClear &= spawners.IfAllWavesClear();
            Debug.Log(spawners.currentWaveIndex);
        }
        Debug.Log(isAllClear + "isAllClear");
        
        if (enemies.Count == 0 && isAllClear )
        {
            Debug.Log("RoomControl: cleared! Opening gate.");
            if (openWhenCleared)
                OpenGate();
        }
    }

    [Header("Clear behaviour")]
    public bool openWhenCleared = true;
    public int GetEnemyCount() => enemies.Count;
    public void OpenGate()
    {
        Debug.Log("RoomControl: Opening gate.");
        gate?.Open();
        CameraBoundary.OnTriggerDoorOpen();
    }

    public void CloseGate()
    {
        Debug.Log("RoomControl: Closing gate.");
        gate?.Close();
    }

    private void OnDisable()
    {
        // 退订所有敌人的死亡事件（保险）
        foreach (var e in enemies)
        {
            if (e != null) e.onEnemyDied -= OnEnemyDied;
        }
    }
}
