using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


[CreateAssetMenu(menuName = "RPG Setup/Chapter Data/New Chapter", fileName = "Chapter - ")]
public class ChapterDataSO :ScriptableObject
{

    public string chapterName;
    
    [Header("该章节下的所有子任务")]
    public List<QuestDataSO> childrenQuests;

    [Header("下一个章节")]
    public ChapterDataSO nextChapterSO;

    [Header("Auto behaviour")]
    public bool autoAcceptFirstQuest = true;

    [Header("章节奖励")]
    public List<ItemDataSO> chapterRewards; 
}
