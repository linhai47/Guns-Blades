using System.Collections.Generic;
using UnityEngine;

public class Player_ChapterManager : MonoBehaviour , ISaveable
{
    [Header("Databases")]
    public ChapterDatabaseSO chapterDatabase;      // 指向你在 Inspector 中创建的 ChapterDatabaseSO
    public QuestDatabaseSO questDatabase;          // 任务数据库（用于比对/查找）

    [Header("Runtime")]
    public int currentChapterIndex = 0;            // 当前章节索引（保存/加载）
    public bool autoAcceptFirstQuest = true;       // 默认行为（可覆盖 ChapterDataSO）

    private Player_QuestManager playerQuestManager;

    private void Awake()
    {

        QuestEvents.OnQuestCompletedById += OnQuestCompleted;
    }

    private void Start()
    {
        playerQuestManager = Player.instance.questManager;
        //LoadChapterProgress();
        //EnsureChapterInitialState();
    }

    private void OnDestroy()
    {
        QuestEvents.OnQuestCompletedById -= OnQuestCompleted;
    }

    private void OnQuestCompleted(string questSaveId)
    {
        Debug.Log($"ChapterManager: received quest completed event: {questSaveId}");
        TryAdvanceChapterIfNeeded();
    }

    public void TryAdvanceChapterIfNeeded()
    {
        var chapterSO = GetCurrentChapterSO();
        if (chapterSO == null) return;

        // 如果某些任务为 null，跳过为 false  
        bool allDone = true;
        foreach (var qSO in chapterSO.childrenQuests)
        {
            if (qSO == null)
            {
                allDone = false;
                break;
            }
            if(playerQuestManager == null)
            {
                playerQuestManager = Player.instance.questManager;
            }
            // 检查 player 的 completedQuests 是否包含此 QuestDataSO
            bool completed = playerQuestManager.completedQuests.Exists(q => q.questDataSO == qSO);
            if (!completed)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            AdvanceChapter();
        }
    }

    private void AdvanceChapter()
    {
        var currentSO = GetCurrentChapterSO();
        Debug.Log($"ChapterManager: chapter completed: {(currentSO != null ? currentSO.chapterName : currentChapterIndex.ToString())}");

        // TODO: 发放章节奖励（如果有）
        GiveChapterRewards(currentSO);

        // 推进索引
        currentChapterIndex++;

        //SaveChapterProgress();

        var next = GetCurrentChapterSO();
        if (next != null)
        {
            Debug.Log($"ChapterManager: entering next chapter: {next.chapterName}");
            // 自动接受第一个任务（按 chapter 配置）
            if (next.childrenQuests != null && next.childrenQuests.Count > 0 && (next.autoAcceptFirstQuest || autoAcceptFirstQuest))
            {
                var firstQS = next.childrenQuests[0];
                // 检查玩家是否已激活或已完成
                bool activeOrCompleted = playerQuestManager.QuestIsActive(firstQS) ||
                                         playerQuestManager.completedQuests.Exists(q => q.questDataSO == firstQS);
                if (!activeOrCompleted)
                {
                    playerQuestManager.AcceptQuest(firstQS);
                    Debug.Log($"ChapterManager: auto-accepted quest {firstQS.questSaveId}");
                }
            }

            // 可选：开始某个章节过场或对话
            // DialogueManager.StartConversation(nextChapterStartConversation, ...)
        }
        else
        {
            Debug.Log("ChapterManager: no more chapters (all completed).");
        }
    }

    private void GiveChapterRewards(ChapterDataSO chapter)
    {
        if (chapter == null) return;
        // 如果你设计了章节奖励，可以在 ChapterDataSO 添加字段并在这里发放
        Debug.Log($"GiveChapterRewards for {chapter.chapterName}");
    }

    private ChapterDataSO GetCurrentChapterSO()
    {
        if (chapterDatabase == null || chapterDatabase.allChapters == null) return null;
        if (currentChapterIndex < 0 || currentChapterIndex >= chapterDatabase.allChapters.Length) return null;
        return chapterDatabase.allChapters[currentChapterIndex];
    }

    public void LoadData(GameData data)
    {
        if (data == null) return;

        currentChapterIndex = data.chapterIndex;

        // 确保索引合法
        if (currentChapterIndex < 0 || chapterDatabase == null ||
            chapterDatabase.allChapters == null ||
            currentChapterIndex >= chapterDatabase.allChapters.Length)
        {
            currentChapterIndex = 0;
        }

        Debug.Log($"[ChapterManager] Loaded Chapter Index: {currentChapterIndex}");

        // 用任务状态来校准章节进度
        TryAdvanceChapterIfNeeded();
    }

    public void SaveData(ref GameData data)
    {
        data.chapterIndex = currentChapterIndex;
        Debug.Log($"[ChapterManager] Saved Chapter Index: {currentChapterIndex}");
    }
}
