using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash;
    [SerializeField] public int MaxStashSize = 18;
    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        var stackableItem = StackableInStash(itemToAdd);
        if (stackableItem != null)
        {
            stackableItem.AddStack();
        }
        else if(materialStash.Count <  MaxStashSize) 
        {
            
            var newItemToAdd = new Inventory_Item(itemToAdd.itemData);

            materialStash.Add(newItemToAdd);
        }

        TriggerUpdateUI();
        materialStash = materialStash.OrderBy(item => item.itemData.name).ToList();
    }

    public Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());


    }
    public void CraftItem(Inventory_Item itemToCraft)
    {
        ConsumeMaterials(itemToCraft);
        playerInventory.AddItem(itemToCraft);

    }
    public bool CanCraftItem(Inventory_Item itemToCraft)
    {
        return HasEnoughMaterials(itemToCraft) && playerInventory.CanAddItem(itemToCraft);
    }
    public void ConsumeMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredItem in itemToCraft.itemData.craftRecipe)
        {
            int amountToConsume = requiredItem.stackSize;

            amountToConsume = amountToConsume - ConsumedMaterialsAmount(playerInventory.itemList, requiredItem);

            if (amountToConsume > 0)
            {
                amountToConsume = amountToConsume - ConsumedMaterialsAmount(itemList, requiredItem);
            }

            if (amountToConsume > 0)
            {
                amountToConsume = amountToConsume - ConsumedMaterialsAmount(materialStash, requiredItem);
            }

        }

    }


    private int ConsumedMaterialsAmount(List<Inventory_Item> itemList, Inventory_Item neededItem)
    {
        ItemDataSO needItemData = neededItem.itemData;
        int amountNeeded = neededItem.stackSize;
        int consumedAmount = 0;

        foreach (var item in itemList)
        {
            if (item.itemData != neededItem.itemData)
            {
                continue;
            }
            int removeAmount = Mathf.Min(item.stackSize, amountNeeded);

            item.stackSize = item.stackSize - removeAmount;
            consumedAmount += removeAmount;

            if (item.stackSize <= 0)
            {
                itemList.Remove(item);
            }
            if (consumedAmount >= amountNeeded)
            {
                break;
            }
        }
        return consumedAmount;
    }
    public bool HasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredMaterial in itemToCraft.itemData.craftRecipe)
        {
            if (GetAvailableAmountOf(requiredMaterial.itemData) < requiredMaterial.stackSize)
                return false;
        }
        return true;

    }
    public int GetAvailableAmountOf(ItemDataSO requiredItem)
    {
        int amount = 0;
        foreach (var item in playerInventory.itemList)
        {
            if (item.itemData == requiredItem)
            {
                amount = amount + item.stackSize;
            }
        }

        foreach (var item in itemList)
        {
            if (item.itemData == requiredItem)
            {
                amount = amount + item.stackSize;
            }
        }

        foreach (var item in materialStash)
        {
            if (item.itemData == requiredItem)
            {
                amount = amount + item.stackSize;
            }
        }

        return amount;
    }
 

    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;


    public void FromPlayerInventoryToStorage(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;


        for (int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);
                playerInventory.RemoveOneItem(item);
                AddItem(itemToAdd);

            }

        }
        TriggerUpdateUI();
    }

    public void FromStorageToPlayerInventory(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                RemoveOneItem(item);
                playerInventory.AddItem(itemToAdd);
            }
        }
        TriggerUpdateUI();
    }

    public void FromStorageToMaterialStash(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; i++)
        {
            if (materialStash.Count < MaxStashSize || StackableInStash(item) != null)
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                RemoveOneItem(item);
                AddMaterialToStash(itemToAdd);
            }
        }
        TriggerUpdateUI();
    }
    public void FromMaterialStashToStorage(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                RemoveOneItemFromStash(item);

                // Ìí¼Óµ½²Ö¿â
                AddItem(itemToAdd);
            }
        }
        TriggerUpdateUI();
    }
    private void RemoveOneItemFromStash(Inventory_Item item)
    {
        var stashItem = materialStash.Find(i => i.itemData == item.itemData);
        if (stashItem != null)
        {
            stashItem.stackSize--;

            if (stashItem.stackSize <= 0)
                materialStash.Remove(stashItem);
        }
    }

   public void RemoveOneItemFromMaterialStash(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = materialStash.Find(item => item == itemToRemove);

        if (itemInInventory.stackSize > 1)
            itemInInventory.RemoveStack();
        else
            materialStash.Remove(itemToRemove);



        //OnInventoryChange?.Invoke();


    }



    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);
        data.storageItems.Clear();

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.saveId;

                if (data.storageItems.ContainsKey(saveId) == false)
                    data.storageItems[saveId] = 0;

                data.storageItems[saveId] += item.stackSize;
            }
        }

        data.storageMaterials.Clear();

        foreach (var item in materialStash)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.saveId;

                if (data.storageMaterials.ContainsKey(saveId) == false)
                    data.storageMaterials[saveId] = 0;

                data.storageMaterials[saveId] += item.stackSize;
            }
        }
    }


    public override void LoadData(GameData data)
    {
        itemList.Clear();
        materialStash.Clear();

        foreach (var entry in data.storageItems)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning("Item not found:" + saveId);
                continue;
            }


            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach (var entry in data.storageMaterials)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning("Item not found:" + saveId);
                continue;
            }


            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddMaterialToStash(itemToLoad);
            }
        }
    }
}
