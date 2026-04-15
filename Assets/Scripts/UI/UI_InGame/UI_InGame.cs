using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;

    [Header("HP")]
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("经验值")]
    [SerializeField] private RectTransform ExpRect;
    [SerializeField] private Slider ExpSlider;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI LVText;


    [Header("Quick Item Slots")]
    [SerializeField] private float yOffsetQuickItemParent = 150;
    [SerializeField] private Transform quickItemOptionsParent;

    private UI_QuickItemSlotOption[] quickItemOptions;
    private UI_QuickItemSlot[] quickItemSlots;

    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();

        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;
        player.playerExp.OnExpChange += UpdateExpBar;

        inventory = player.inventory;
        inventory.OnInventoryChange += UpdateQuickSlotUI;
        inventory.OnQuickSlotUsed += PlayQuickSlotFeedback;
    }



    public void PlayQuickSlotFeedback(int slotNumber) => quickItemSlots[slotNumber].SimulateButtonFeedBack();

    public void UpdateQuickSlotUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;

        for (int i = 0; i < quickItems.Length; i++)
        {
            quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);
        }


    }


    public void OpenQuickItemOptions(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        if (quickItemOptions == null)
        {
            quickItemOptions = quickItemOptionsParent.GetComponentsInChildren<UI_QuickItemSlotOption>(true);


        }
        List<Inventory_Item> consumables = inventory.itemList.FindAll(item => item.itemData.itemType == ItemType.Consumable);
        Debug.Log(consumables.Count);
        for (int i = 0; i < quickItemOptions.Length; i++)
        {
            if (i < consumables.Count)
            {
                quickItemOptions[i].gameObject.SetActive(true);

                quickItemOptions[i].SetupOption(quickItemSlot, consumables[i]);

            }
            else
            {
                quickItemOptions[i].gameObject.SetActive(false);
            }

        }
        quickItemOptionsParent.position = targetRect.position + Vector3.up * yOffsetQuickItemParent;

    }
    public void HideQuickItemOptions() => quickItemOptionsParent.position = new Vector3(0, 9999);

    public UI_SkillSlot GetNewSkillSlot()
    {
        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        foreach (var slot in skillSlots)
        {

            slot.gameObject.SetActive(true);

            // 找到空位
            if (slot.skillType == SkillType.None) return slot;



        }
        return null;
    }
    public void ResetSkillInGame()
    {
        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        foreach(var slot in skillSlots)
        {
            slot.ResetSkillSlot();
        }
    }

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {

        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        foreach (var slot in skillSlots)
        {
           
                slot.gameObject.SetActive(true);
                

           if(slot.skillType ==skillType )     return slot;

            

        }
        return null;

    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetcurrentHealth());
        float maxHealth = player.stats.GetMaxHealth();
        //float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);

        //if (sizeDifference > .1f)
        //    healthRect.sizeDelta = new Vector2(maxHealth * .2f, healthRect.sizeDelta.y);


        healthText.text = currentHealth + "/" + maxHealth;

        healthSlider.value = player.health.GetHealthPercent();


    }


    private void UpdateExpBar()
    {
        float currentExp = Mathf.RoundToInt(player.playerExp.exp);
        float nextLevelExp = player.playerExp.nextLevelNeedExp;
        expText.text = currentExp + "/" + nextLevelExp;
        ExpSlider.value =player.playerExp.getExpPercent();
        LVText.text= player.playerExp.playerLevel.ToString();

    }
}
