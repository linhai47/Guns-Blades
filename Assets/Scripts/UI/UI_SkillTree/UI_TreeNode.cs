using System.ComponentModel;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectionHandler connectHandler;


    [Header("Unlock details")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill details")]
    public SkillDataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private Color skillLockedColor;
    private string lockedColorHex = "#9F9797";
    private Color lastColor;






    private void Start()
    {

        if (isUnlocked == false)
            UpdateIconColor(GetColorByHex(lockedColorHex));
        UnlockDefaultSkills();


    }

    public void UnlockDefaultSkills()
    {

        GetNeededComponents();
        if (skillData.unlockedByDefault)
        {
            Unlock();
        }
    }
    private void GetNeededComponents()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>(true);
        connectHandler = GetComponent<UI_TreeConnectionHandler>();
    }
    public void Refund()
    {
        if (isUnlocked == false || skillData.unlockedByDefault)
        {
            return;
        }

        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));
        skillTree.AddSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(false);

    }
    public void Unlock(bool isLoading = false)
    {
        if (isUnlocked)
        {
            Debug.Log("Skill is already unlocked.");
            return;
        }

        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();


       if(!isLoading) skillTree.RemoveSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(true);

      if(skillData!=null)  skillTree.skillManager.GetSKillByType(skillData.skillData.skillType).SetSkill(skillData);

    }


    public void UnlockWithSaveData()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();
        connectHandler.UnlockConnectionImage(true);

    }

    private bool CanBeUnlocked()
    {
        if (isUnlocked || isLocked)
        {
            return false;

        }
        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
        {
            return false;
        }



        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }
        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)
            {
                return false;
            }
        }
        return true;
    }
    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
            node.LockChildNodes();
        }
    }

    public void LockChildNodes()
    {
        isLocked = true;
        foreach (var node in connectHandler.GetChildNodes())
        {
            node.LockChildNodes();
        }
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null) return;
        lastColor = skillIcon.color;
        skillIcon.color = color;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
     
        if (CanBeUnlocked())
        {
            Unlock();
        }
        else if (isLocked)
        {
            ui.skillToolTip.LockedSkillEffect();
        }



    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, skillData, this);

        if (isLocked || isUnlocked)
            return;



        ToggleNodeHighlight(true);





    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);
        ui.skillToolTip.StopLockedSkillEffect();
        if (isLocked || isUnlocked)
            return;
        ToggleNodeHighlight(false);
    }
    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .9f; highlightColor.a = 1;
        Color colorToApply = highlight ? highlightColor : lastColor;

        UpdateIconColor(colorToApply);
    }
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);
        return color;
    }
    private void OnDisable()
    {
        if (isLocked)
        {
            UpdateIconColor(GetColorByHex(lockedColorHex));
        }
        if (isUnlocked)
        {
            UpdateIconColor(Color.white);
        }
    }

    private void OnValidate()
    {
        if (skillData == null)
        {
            return;
        }
        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }
}
