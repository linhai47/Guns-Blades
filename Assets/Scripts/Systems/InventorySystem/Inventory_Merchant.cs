using System.Collections.Generic;
using UnityEngine;

public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player inventory;

    [SerializeField] private UI_ShopAvatar shopAvatar;

    [SerializeField] private ItemListDataSO shopData;
    [SerializeField] private int minItemsAmount = 2;
    public ItemType type = ItemType.Material;
    protected override void Awake()
    {
        base.Awake();
   
    }
    private void Start()
    {
      
      
        FillShopList(type);
    }


    public void TryBuyItem(Inventory_Item itemToBuy, bool buyFullStack)
    {
        TriggerUpdateUI();
        if (itemToBuy == null)
        {
            Debug.LogError("itemToBuy 是 null，检查 UI_MerchantSlot 传进来的参数");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("playerInventory 没有初始化");
            return;
        }

        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;
        for(int i = 0; i < amountToBuy; i++)
        {
            if(inventory.gold < itemToBuy.buyPrice)
            {
                Debug.Log("Not Enough Money");
            }

            if(itemToBuy.itemData.itemType == ItemType.Material)
            {
                inventory.storage.AddMaterialToStash(itemToBuy);
            }
            else
            {
                if (inventory.CanAddItem(itemToBuy))
                {
                    Inventory_Item buy_item = new Inventory_Item(itemToBuy.itemData); 
                    inventory.AddItem(buy_item);

                   
                }



            }
            inventory.gold -= itemToBuy.buyPrice;
            RemoveOneItem(itemToBuy);
        }
        TriggerUpdateUI();
    }

    public void TrySellItem(Inventory_Item itemToSell, bool sellFullStack)
    {
        int amountToSell = sellFullStack ? itemToSell.stackSize : 1;

        for (int i = 0; i < amountToSell; i++)
        {
            int sellPrice = Mathf.FloorToInt(itemToSell.sellPrice);

            inventory.gold = inventory.gold + sellPrice;
            inventory.RemoveOneItem(itemToSell);
        }

        TriggerUpdateUI();
    }

    public void FillShopList(ItemType itemType )
    {
        itemList.Clear();
        //Debug.Log(itemType);
        type = itemType;
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach (var item in shopData.itemList)
        {
            if (itemType != item.itemType && itemType != ItemType.None) continue;
            int randomStacksize  = Random.Range(item.minStackSizeAtShop , item.maxStackSizeAtShop+1);
            int finalStack = Mathf.Clamp(randomStacksize, 1, item.maxStackSize);

            Inventory_Item itemToAdd = new Inventory_Item(item);
            itemToAdd.stackSize = finalStack;

            possibleItems.Add(itemToAdd);

        }

        int randomItemAmount = Random.Range(minItemsAmount, maxInventorySize + 1);
        int finalAmount = Mathf.Clamp(randomItemAmount, 1, possibleItems.Count);

        for (int i = 0; i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];

            if (CanAddItem(item))
            {
                possibleItems.Remove(item);
                AddItem(item);
            }
        }


        TriggerUpdateUI();
    }

    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;
}
