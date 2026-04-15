using UnityEngine;

public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSO craftData;
    [SerializeField] private UI_CraftAvatar craftAvatar;
    private UI_CraftSlot[] craftSlots;

    [SerializeField] private ItemType type;

    private void Awake()
    {
        craftAvatar = FindFirstObjectByType<UI_CraftAvatar>();
    }
    public void SetCraftSlots(UI_CraftSlot[] craftSlots) => this.craftSlots = craftSlots;

    public void UpdateCraftSlots()
    {
        if (craftSlots == null)
        {
            Debug.Log("You need to assign craft list data!");
            return;
        }

        foreach (var slot in craftSlots)
        {
            slot.gameObject.SetActive(false);
        }

        craftAvatar.ChangeType(type);


        for (int i = 0; i < craftData.itemList.Length; i++)
        {
            ItemDataSO itemData = craftData.itemList[i];
        


            craftSlots[i].gameObject.SetActive(true);
            craftSlots[i].SetupButton(itemData);


        }
    }
}
