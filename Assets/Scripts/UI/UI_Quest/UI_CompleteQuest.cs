using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

public class UI_CompleteQuest : MonoBehaviour
{
    private Player_QuestManager questManager;

    private List<UI_CompleteQuestSlot> questSlots = new List<UI_CompleteQuestSlot>();
    public UI_CompleteQuestSlot slotPrefab;
    public Transform slotParent;
    private void Awake()
    {
        questManager = Player.instance.questManager;

       
    }

    private void OnEnable()
    {
        List<QuestData> quests = questManager.completedQuests;

        foreach (var slot in questSlots)
        {
            Destroy(slot.gameObject);
        }
        questSlots.Clear();

        for (int i = 0; i < quests.Count; i++)
        {
            var slot = Instantiate(slotPrefab, slotParent);
            slot.gameObject.SetActive(true);
            slot.SetupCompleteQuestSlot(quests[i]);
            questSlots.Add(slot);
        }

        if (questSlots.Count > 0)
            questSlots[0].SetupPreviewBTN();

    }
}
