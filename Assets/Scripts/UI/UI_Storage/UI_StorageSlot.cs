using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private Inventory_Storage storage;

    public enum StorageSlotType { StorageSlot, PlayerInventorySlot , materialSlot }
    public StorageSlotType slotType;
    public void SetStorage(Inventory_Storage storage) => this.storage = storage;



    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        bool transferFullStack = Input.GetKey(KeyCode.LeftControl);


        if (slotType == StorageSlotType.StorageSlot)
        {
            if(itemInSlot.itemData.itemType== ItemType.Material)
            {
                storage.FromStorageToMaterialStash(itemInSlot,transferFullStack);
            }
            else storage.FromStorageToPlayerInventory(itemInSlot, transferFullStack);
        }
        if (slotType == StorageSlotType.PlayerInventorySlot)
        {
            storage.FromPlayerInventoryToStorage(itemInSlot, transferFullStack);
        }
        if(slotType  == StorageSlotType.materialSlot)
        {
            storage.FromMaterialStashToStorage(itemInSlot, transferFullStack);
        }

        ui.itemToolTip.ShowToolTip(false, null);
    }
}
