using UnityEngine;
using PixelCrushers.DialogueSystem;
public class Player_DialogueManager : MonoBehaviour
{
    public Player_QuestManager player_QuestManager;

    //public Transform npcTalking;

    private void Awake()
    {
        player_QuestManager = GetComponent<Player_QuestManager>();
    }
    private string PickConversationForNpc(Transform npc)
    {

        Object_NPC NPC = npc.GetComponent<Object_NPC>();

        foreach (var rec in player_QuestManager.activeQuests)
        {
            var questDataSO = rec.questDataSO; // 假定 QuestRecord 含对 QuestDataSO 的引用
            if (questDataSO == null) continue;

            // 你可能需要判断这个 questSO 是否与当前 npc 相关，比如匹配 questSO.questTargetId 或某个 NPC tag
            if (NPC.Npcname == questDataSO.questTargetId)
            {
                // 假定你在 SO 中保存了会话名（string entryConversationTitle）
                if (!string.IsNullOrEmpty(questDataSO.questName))
                    return questDataSO.questName;
            }
        }

        // 如果没找到，则返回 null
        return null;
    }

    public void TryStartDialogueFor(Transform npcTalking)
    {
        // 1) 根据玩家任务挑选合适的对话名称（你自己实现匹配逻辑）
        string conversationToPlay = PickConversationForNpc(npcTalking);

        Debug.Log("触发任务对话" +conversationToPlay);
        var conversation = DialogueManager.MasterDatabase.GetConversation(conversationToPlay);
        if (conversation == null)
        {
            Debug.LogError($"[QuestDialog] 会话 '{conversationToPlay}' 在数据库中未找到！");
            return;
        }

        string actorName = npcTalking.name; 
        Actor actor = DialogueManager.MasterDatabase.GetActor(actorName);
        Debug.Log(actorName);

        if (actor != null)
            Debug.Log($"Found Actor in database: {actor.Name}");
        else
            Debug.LogWarning($"Actor '{actorName}' not found in Dialogue Database!");



        if (!string.IsNullOrEmpty(conversationToPlay))
        {
            Debug.Log($"[QuestDialog] Starting quest conversation '{conversationToPlay}' for NPC ");
          
            JumpToConversation(conversationToPlay);

            return;
        }

    }

    private static void JumpToConversation(string conversationToPlay)
    {
        DialogueManager.StopConversation(); // 立即结束当前会话
        DialogueManager.StartConversation(conversationToPlay);
    }
}
