using TMPro;
using UnityEngine;

public class UI_Merchant : MonoBehaviour
{
    private Inventory_Player inventory;
    [SerializeField] private Inventory_Merchant merchant;


    [SerializeField] private TextMeshProUGUI goldText;
    [Space]
    [SerializeField] private UI_ItemSlotParent merchantSlots;

    public void SetupMerchantUI(Inventory_Merchant merchant , Inventory_Player inventory)
    {
        this.merchant = merchant;
        this.inventory = inventory;

        this.merchant.OnInventoryChange += UpdateSlotUI;
        this.inventory.OnInventoryChange += UpdateSlotUI;
        UpdateSlotUI();
        UI_MerchantSlot[] merchantSlots = GetComponentsInChildren <UI_MerchantSlot>();
        foreach (var slot in merchantSlots)
        {
            slot.SetupMerchantUI(merchant);
            
        }


    }

    private void UpdateSlotUI()
    {
        if (inventory == null)
        {
            Debug.Log("inventory is null");
            return;

        }

        merchantSlots.UpdateSlots(merchant.itemList);
        

        goldText.text = inventory.gold.ToString() + "g.";
    }
}
