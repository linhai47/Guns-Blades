using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_DungeonSlot : MonoBehaviour
{
    private DungeonDataSO dungeonData;



    public Image icon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public UnityEvent<DungeonDataSO> onSlotClicked;
    private Button _button;
    public GameObject selectedHighlight;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    public void SetUpSlot(DungeonDataSO data)
    {
        dungeonData = data;
        if (data == null)
        {
            if (icon) icon.sprite = null;
            if (titleText) titleText.text = "";
            if (levelText) levelText.text = "";
            gameObject.SetActive(false); // 如果你希望空 slot 隐藏
            return;
        }

        gameObject.SetActive(true);
        if (icon) icon.sprite = data.icon;
        if (titleText) titleText.text = data.dungeonName;
        if (levelText) levelText.text = $"推荐等级 Lv {data.recommendedLevel}";

    }
    public void HandleClick()
    {
        if (dungeonData == null) return;
        onSlotClicked?.Invoke(dungeonData);
    }

    /// <summary>
    /// 外部调用以显示/隐藏选中效果
    /// </summary>
    public void SetSelected(bool selected)
    {
        if (selectedHighlight != null) selectedHighlight.SetActive(selected);
    }

    /// <summary>
    /// 取回当前绑定的数据
    /// </summary>
    public DungeonDataSO GetData()
    {
        return dungeonData;
    }
}
