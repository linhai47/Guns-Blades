using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CompleteQuestSlot : MonoBehaviour
{
    private QuestData questInSlot;
    private UI_CompleteQuestPreview questPreview;


    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] questRewardPreview;


    public void SetupCompleteQuestSlot(QuestData questToSetup)
    {
        questPreview = transform.root.GetComponentInChildren<UI_CompleteQuestPreview>();

        questInSlot = questToSetup;

        questName.text = questToSetup.questDataSO.questName;

        Inventory_Item[] reward = questToSetup.questDataSO.rewardItems;


        foreach (var previewIcon in questRewardPreview)
        {
            previewIcon.gameObject.SetActive(false);



        }
        for (int i = 0; i < reward.Length; i++)
        {
            if (reward[i] == null) continue;

            Image preview = questRewardPreview[i];

            preview.gameObject.SetActive(true);
            preview.sprite = reward[i].itemData.itemIcon;
            preview.GetComponentInChildren<TextMeshProUGUI>().text = reward[i].stackSize.ToString();
        }

    }


    public void SetupPreviewBTN()
    {
        questPreview.SetupQuestPreview(questInSlot);

    }
}
