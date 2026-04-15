using UnityEngine;
using UnityEngine.Rendering;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] uiStatSlots;
    private Inventory_Player inventory;

    private void Awake()
    {
        uiStatSlots = GetComponentsInChildren<UI_StatSlot>();
        inventory = FindAnyObjectByType<Inventory_Player>();

        inventory.OnInventoryChange += UpdateStatsUI;
    }

    private void Start()
    {
        UpdateStatsUI();
    }
    private void UpdateStatsUI()
    {
        foreach (var statSlot in uiStatSlots)
        {
            statSlot.UpdateStatValue();
        }
    }
}
