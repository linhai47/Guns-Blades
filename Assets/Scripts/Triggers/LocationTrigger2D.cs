using UnityEngine;

/// <summary>
/// 当玩家进入触发区时，向 Player_QuestManager 报告位置到达（调用 AddProgress）。
/// 适配你现有的 AddProgress(NPCname, int) 接口，因此地点可以暂时用 NPCname 表示。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LocationTrigger2D : MonoBehaviour
{
    [Tooltip("复用 NPCname 枚举来表示地点（临时方案）。")]
    public NPCname placeId;

    [Tooltip("一次触发增加的进度量")]
    public int progressAmount = 1;

    [Tooltip("是否只触发一次（进入一次后禁用/销毁）")]
    public bool singleUse = true;

    [Tooltip("可选：触发时是否需要按键交互（例如按E）")]
    public bool requireInteractKey = false;

    private bool used = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是不是玩家（根据你的项目Player标签/组件调整）
        var player = other.GetComponent<Player>();
        if (player == null) return;
        Debug.Log("Player Detected");
        TryTriggerForPlayer(player);
    }

    private void TryTriggerForPlayer(Player player)
    {
        if (used && singleUse) return;
        if (requireInteractKey)
        {
            // 如果需要交互键，则这里不直接触发（你可以显示UI提示并等待按键）
            // 简化：不实现交互逻辑，只提示
            Debug.Log("LocationTrigger: require interact key - implement key handling in your UI/Player controller.");
            return;
        }

        var qm = player.questManager;
        if (qm == null)
        {
            Debug.LogWarning("LocationTrigger: player's questManager is null.");
            return;
        }
        Debug.Log("Add progress");
        qm.AddProgress(placeId, progressAmount);
        used = true;

        if (singleUse)
        {
            // 两种选择：禁用组件或销毁对象
            gameObject.SetActive(false);
            // or: Destroy(gameObject);
        }
    }
}
