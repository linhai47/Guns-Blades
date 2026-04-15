using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonSelectUI : MonoBehaviour
{
    [Header("Data")]
    public DungeonDatabaseSO database;

    public UI_DungeonSlot[] dungeonSlots;

    [Header("Detail UI")]
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDesc;
    public TextMeshProUGUI detailReq;

    public Button startButton;
    [Header("References")]
    public DungeonManager dungeonManager;
    public Player player;

    private DungeonDataSO _currentSelection;

    private void Start()
    {
        PopulateSlots();
        if (startButton != null) startButton.onClick.AddListener(OnStartButton);
        UpdateDetail(null);
    }

    private void OnDestroy()
    {
        if (startButton != null) startButton.onClick.RemoveListener(OnStartButton);
    }
    /// <summary>
    /// 把 database 填充到 slot 池里（按顺序）
    /// </summary>
    private void PopulateSlots()
    {
        if (database == null || dungeonSlots == null) return;

        var dungeons = database.dungeons;
        int count = Mathf.Min(dungeons.Length, dungeonSlots.Length);

        // 初始化 slots
        for (int i = 0; i < dungeonSlots.Length; i++)
        {
            if (i < count)
            {
                dungeonSlots[i].SetUpSlot(dungeons[i]);
                dungeonSlots[i].onSlotClicked.RemoveAllListeners();
                dungeonSlots[i].onSlotClicked.AddListener(OnSlotClicked);
                dungeonSlots[i].gameObject.SetActive(true);
            }
            else
            {
                // 没有数据的 slot 隐藏或清空
                dungeonSlots[i].SetUpSlot(null);
                dungeonSlots[i].onSlotClicked.RemoveAllListeners();
            }
            dungeonSlots[i].SetSelected(false);
        }
    }

    private void OnSlotClicked(DungeonDataSO data)
    {
        // 记录当前选中并更新 UI
        _currentSelection = data;
        UpdateSlotsSelection();
        UpdateDetail(data);
    }

    private void UpdateSlotsSelection()
    {
        foreach (var s in dungeonSlots)
        {
            var d = s.GetData();
            s.SetSelected(d != null && _currentSelection != null && d.dungeonID == _currentSelection.dungeonID);
        }
    }

    private void UpdateDetail(DungeonDataSO d)
    {
        if (d == null)
        {
            if (detailIcon) detailIcon.sprite = null;
            if (detailName) detailName.text = "";
            if (detailDesc) detailDesc.text = "";
            if (detailReq) detailReq.text = "";

            if (startButton) startButton.interactable = false;
            return;
        }

        if (detailIcon) detailIcon.sprite = d.icon;
        if (detailName) detailName.text = d.dungeonName;
        if (detailDesc) detailDesc.text = d.dungeonDescription;
        if (detailReq) detailReq.text = $"  推荐 Lv {d.recommendedLevel  }";
     

        // 校验是否满足进入条件
        string reason;
        bool canEnter = CanEnter(d, out reason);
        if (startButton) startButton.interactable = canEnter;
        // TODO: 把 reason 展示给玩家（tooltip 或小文本）
    }

    private bool CanEnter(DungeonDataSO d, out string reason)
    {
        reason = "";
        if (player == null) { reason = "无玩家数据"; return false; }
 
      
        return true;
    }

    private void OnStartButton()
    {
        if (_currentSelection == null)
        {
            Debug.LogWarning("未选择副本");
            return;
        }

        string reason;
        if (!CanEnter(_currentSelection, out reason))
        {
            Debug.LogWarning("无法进入副本: " + reason);
            // show UI tip
            return;
        }


        dungeonManager?.EnterDungeon(_currentSelection);
    }

}
