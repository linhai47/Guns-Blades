using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    private Inventory_Player inventory;
    private UI_CraftPreview craftPreviewUI;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButtons;

    public void SetupCraftUI(Inventory_Storage storage)
    {
        inventory = storage.playerInventory;
       

        craftPreviewUI = GetComponentInChildren<UI_CraftPreview>();
        craftPreviewUI.SetupCraftPreview(storage);
        SetupCraftListButtons();
    }

    private void SetupCraftListButtons()
    {
  
        craftSlots = GetComponentsInChildren<UI_CraftSlot>(true);

        craftListButtons = GetComponentsInChildren<UI_CraftListButton>(true);


        foreach (var slot in craftSlots)
        {

            slot.gameObject.SetActive(false);
        }
        foreach (var button in craftListButtons)
        {
            button.SetCraftSlots(craftSlots);
        }




    }
}
