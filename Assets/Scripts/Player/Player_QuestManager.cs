using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public List<QuestData> activeQuests;
    public List<QuestData> completedQuests;
    private Entity_DropManager dropManager;
    private Inventory_Player inventory;

    public QuestDataSO[] availableQuests;
    public Object_NPC questNPC;
    [Header("QUEST DATABASE")]
    [SerializeField] private QuestDatabaseSO questDatabase;



    private void Awake()
    {

        inventory = GetComponent<Inventory_Player>();
        dropManager = GetComponent<Entity_DropManager>();
    }


    public void TryGetRewardFrom()
    {
        Debug.Log("Try get reward");
        Debug.Log(activeQuests.Count);
        List<QuestData> getRewardQuests = new List<QuestData>();
        if (questNPC == null)
        {
            Debug.LogError("questNPC is null!");
            return;
        }
        RewardType npcType = questNPC.rewardNpc;


        foreach (var quest in activeQuests)
        {
            Debug.Log(npcType.ToString() + quest.questDataSO.rewardType);
            // DELIVER Items IF CAN
            if (quest.questDataSO.questType == QuestType.Delivery)
            {
                var requiredItem = quest.questDataSO.itemToDeliver;
                var requiredAmount = quest.questDataSO.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                {
                    inventory.RemoveItemAmount(requiredItem, requiredAmount);

                    quest.AddQuestProgress(requiredAmount);
                }


            }
            if (quest.CanGetReward() && quest.questDataSO.rewardType == npcType)
            {

                getRewardQuests.Add(quest);

            }

        }
        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSO);
            CompleteQuest(quest);

        }

    }

    private void GiveQuestReward(QuestDataSO questDataSO)
    {
        foreach (var item in questDataSO.rewardItems)
        {
            if (item == null || item.itemData == null) continue;

            for (int i = 0; i < item.stackSize; i++)
            {
                dropManager.CreateItemDrop(item.itemData);


            }



        }


    }
    public bool HasCompletedQuest()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            QuestData quest = activeQuests[i];

            if (quest.questDataSO.questType == QuestType.Delivery)
            {
                var requiredItem = quest.questDataSO.itemToDeliver;
                var requiredAmount = quest.questDataSO.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                    return true;
            }

            if (quest.CanGetReward())
                return true;
        }

        return false;
    }


    public void AddProgress(NPCname questTargetId, int amount = 1)
    {
        if(activeQuests.Count==0) return;
        List<QuestData> getRewardQuests = new List<QuestData>();
        foreach (var quest in activeQuests)
        {
            Debug.Log(quest.questDataSO.questTargetId);
            if (quest.questDataSO.questTargetId != questTargetId) continue;

            if (quest.CanGetReward() == false)
                quest.AddQuestProgress(amount);

            if (quest.questDataSO.rewardType == RewardType.None && quest.CanGetReward())
            {
                getRewardQuests.Add(quest);

            }

        }

        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSO);
            CompleteQuest(quest);

        }

    }
    public int GetQuestProgress(QuestData questToCheck)
    {
        QuestData quest = activeQuests.Find(q => q == questToCheck);

        return quest != null ? quest.currentAmount : 0;


    }


    public void AcceptQuest(QuestDataSO questDataSO)
    {
        activeQuests.Add(new QuestData(questDataSO));
        RemoveFromAvailableQuests(questDataSO);
    }
    public void CompleteQuest(QuestData questData)
    {
        completedQuests.Add(questData);
        activeQuests.Remove(questData);

        if (questData != null && questData.questDataSO != null)
        {
            QuestEvents.RaiseQuestCompleted(questData.questDataSO.questSaveId);
        }
        foreach (var quest in questData.questDataSO.ChildrenQuestSO)
        {
            QuestData toAddQuest = new QuestData(quest);

            activeQuests.Add(toAddQuest);
            Debug.Log(toAddQuest.ToString());
        }

        

    }
    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return false;


        return activeQuests.Find(q => q.questDataSO == questToCheck) != null;
    }
    private void RemoveFromAvailableQuests(QuestDataSO questToRemove)
    {
        if (availableQuests == null || availableQuests.Length == 0 || questToRemove == null) return;

        var list = new List<QuestDataSO>(availableQuests);
        bool removed = list.RemoveAll(q => q == questToRemove) > 0; // 使用 RemoveAll 防止重复项残留

        if (removed)
        {
            availableQuests = list.ToArray();
            Debug.Log($"Removed quest '{questToRemove.questSaveId}' from availableQuests.");
        }
        else
        {
            // 没找到也无需报错，可能来自别的来源
            Debug.Log($"Quest '{questToRemove.questSaveId}' was not found in availableQuests.");
        }
    }
    public void LoadData(GameData data)
    {
        activeQuests.Clear();
        completedQuests.Clear();
        foreach (var entry in data.activeQuests)
        {
            string questSaveId = entry.Key;
            int progress = entry.Value;


            QuestDataSO questDataSO = questDatabase.GetQuestById(questSaveId);

            if (questDataSO == null)
            {
                Debug.Log(questSaveId + " was not found in the database");
                continue;
            }
            QuestData questToLoad = new QuestData(questDataSO);
            questToLoad.currentAmount = progress;

            activeQuests.Add(questToLoad);
        }

        foreach(var entry in data.completedQuests)
        {
            string questSaveId = entry.Key;
            bool progress = entry.Value;
            QuestDataSO questDataSO = questDatabase.GetQuestById(questSaveId);

            if (questDataSO == null)
            {
                Debug.Log(questSaveId + " was not found in the database");
                continue;
            }
            QuestData questToLoad = new QuestData(questDataSO);
            questToLoad.canGetReward = progress;

            completedQuests.Add(questToLoad);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.activeQuests.Clear();
        data.completedQuests.Clear();

        foreach (var quest in activeQuests)
        {
            data.activeQuests.Add(quest.questDataSO.questSaveId, quest.currentAmount);



        }

        foreach (var quest in completedQuests)
        {
            data.completedQuests.Add(quest.questDataSO.questSaveId, quest.canGetReward);

        }
    }
}
