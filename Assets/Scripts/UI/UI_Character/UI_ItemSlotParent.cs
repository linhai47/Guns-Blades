using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UI_ItemSlotParent : MonoBehaviour
{
    private UI_ItemSlot[] ItemSlots;
   
    public void UpdateSlots(List <Inventory_Item> itemList) 
    {
        if(ItemSlots == null)
        {
            ItemSlots = GetComponentsInChildren<UI_ItemSlot>();
        }

        for(int i = 0; i < ItemSlots.Length; i++)
        {
          if(i < itemList.Count)
            {
                ItemSlots[i].UpdateSlot(itemList[i]);
            }
            else
            {

                ItemSlots[i].UpdateSlot(null);
            }
        }


    }

}
