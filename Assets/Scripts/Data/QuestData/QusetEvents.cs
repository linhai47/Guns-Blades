// QuestEvents.cs
using System;
public static class QuestEvents
{
    // 当某个 quest 完成（CompleteQuest 被调用时），派发事件
    public static event Action<string> OnQuestCompletedById; // 传 questSaveId

    public static void RaiseQuestCompleted(string questSaveId)
    {
        OnQuestCompletedById?.Invoke(questSaveId);
    }
}
