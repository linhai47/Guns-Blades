using UnityEngine;
[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect / Refund All Skills", fileName = "Item effect data -  Refund All Skills")]
public class ItemEffect_RefundSkillPoints : ItemEffect_DataSO
{
    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        Entity_SkillManager entity_SkillManager = FindFirstObjectByType<Entity_SkillManager>();
        ui.skillTreeUI.RefundAllSkills();
        ui.inGameUI.ResetSkillInGame();
       for(int i = 0; i < entity_SkillManager.playerSkillSlots.Length; i++)
        {
            entity_SkillManager.playerSkillSlots[i] = SkillType.None;
        }
    }
}
