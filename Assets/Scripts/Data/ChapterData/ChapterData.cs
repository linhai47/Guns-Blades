using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ChapterData : MonoBehaviour
{
    public ChapterDataSO chapterDataSO;
    
    public List<QuestDataSO> chapterQuest;

    public bool CanGoToNextChapter(List<QuestDataSO> completedQuests)
    {
        if (chapterDataSO == null || chapterDataSO.childrenQuests == null) return false;

        foreach (var quest in chapterDataSO.childrenQuests)
        {
            if (!completedQuests.Contains(quest))
            {
                // 只要有一个子任务没完成，就不能进入下一章
                return false;
            }
        }

        return true;
    }

    public ChapterData(ChapterDataSO chapterDataSO)
    {
        this.chapterDataSO = chapterDataSO;
    }
}
