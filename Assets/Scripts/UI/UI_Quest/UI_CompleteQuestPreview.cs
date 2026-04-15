using TMPro;
using UnityEngine;

public class UI_CompleteQuestPreview : MonoBehaviour
{
    private Player_QuestManager questManager;
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;



    public void SetupQuestPreview(QuestData questData)
    {
        questManager = Player.instance.questManager;

        QuestDataSO questSO = questData.questDataSO;

        questName.text = questSO.name;
        description.text = questSO.description;

        progress.text = questSO.questGoal + " 任务已完成"  ;

        foreach (var obj in questRewardSlots)
        {
            obj.gameObject.SetActive(false);
        }

        for (int i = 0; i < questSO.rewardItems.Length; i++)
        {
            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateSlot(questSO.rewardItems[i]);
        }

    }
}
