using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour , ISaveable
{
    [SerializeField] private int skillPoints;

    [SerializeField] private TextMeshProUGUI skillPointsText;

    [SerializeField] private UI_TreeConnectionHandler[] parentNodes;
    private UI_TreeNode[] allTreeNodes;


    public Entity_SkillManager skillManager { get; private set; }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    public void RemoveSkillPoints(int cost)
    {
        skillPoints -= cost;
        UpdateSkillPointsUI();
    }
    public void AddSkillPoints(int points)
    {
        skillPoints += points;
        UpdateSkillPointsUI();
    }

    private void Start()
    {
        UpdateAllConnections();
        UpdateSkillPointsUI();
    }

    private void UpdateSkillPointsUI()
    {
        skillPointsText.text ="技能点数 :" +  skillPoints.ToString();
    }

    public void UnlockDefaultSkills()
    {
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        skillManager = FindFirstObjectByType<Entity_SkillManager>();

        foreach (var node in allTreeNodes)
        {
            node.UnlockDefaultSkills();
        }
    }


    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();
        foreach (var node in skillNodes)
        {
            node.Refund();
        }
    }

    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {

            node.UpdateAllConnections();
        }
    }

    public void LoadData(GameData data)
    {
        skillPoints = data.skillPoints;

     

        foreach (var skill in skillManager.allSkills)
        {
            if (data.playerSkills.TryGetValue(skill.GetSkillType().ToString(), out SkillType skillType))
            {
                foreach (var node in allTreeNodes)
                {
                    if(node.skillData.skillData.skillType == skillType)
                    {

                        node.Unlock(true);
                    }
                   

                }

            }

        }
    }

    public void SaveData(ref GameData data)
    {
        
        data.skillPoints = skillPoints;
        data.skillTreeUI.Clear();
        data.playerSkills.Clear();
        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;
            data.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach (var skill in skillManager.playerSkillSlots)
        {
            string skillTypeName = skill.ToString();
            data.playerSkills[skillTypeName] = skill;

        }
    }
}
