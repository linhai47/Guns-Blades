using UnityEditor;
using UnityEngine;


public enum RewardType { MeetNPC, None }

public enum QuestType { Kill, Talk, Delivery , Scene }

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    public string questSaveId;
    [Space]
    public QuestType questType;
    public string questName;

    [TextArea] public string description;
    [TextArea] public string questGoal;



    public NPCname questTargetId;
    public int requiredAmount;
    public ItemDataSO itemToDeliver;
    [Header("Reward")]

    public RewardType rewardType;
    public Inventory_Item[] rewardItems;

    [Header("Children Quest")]
    public QuestDataSO[] ChildrenQuestSO;


    private void OnValidate()
    {
#if UNITY_EDITOR        
        string path = AssetDatabase.GetAssetPath(this);
        questSaveId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
