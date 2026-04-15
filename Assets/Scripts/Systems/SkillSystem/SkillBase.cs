using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public Entity_SkillManager skillManager { get; private set; }
    public Player player { get; private set; }

    
    public DamageScaleData damageScaleData { get; private set; }

    public BasicSkillType BasicSkillType { get; private set; }

    [SerializeField] protected string triggerName;
  
    [Header("Skill details")]
    [SerializeField] protected float cooldown;
    [SerializeField] protected BasicSkillType basicSkillType;
    [SerializeField] protected SkillType skillType;

    private float lastTimeUsed;
    private bool isCast = false;

    protected virtual void Awake()
    {
        skillManager = GetComponentInParent<Entity_SkillManager>();
        player = GetComponentInParent<Player>();
        lastTimeUsed = lastTimeUsed - cooldown;
        damageScaleData = new DamageScaleData();
    }
   
    public virtual void TryUseSkill()
    {
        if (!CanUseSkill() || skillManager.globalCastingTime > 0f ) return;
      
        Cast();
        SetSkillOnCooldown();
    }

    public void Cast()
    {
        //Debug.Log("Cast");
        
      
        skillManager.globalCastingTime= player.castingState.GetDuration(triggerName);
       
        player.castingState.SetTrigger(triggerName);
        player.stateMachine.ChangeState(player.castingState);
    }



    public BasicSkillType GetBasicSkillType() => basicSkillType;

    public SkillType GetSkillType() => skillType;
    public virtual bool CanUseSkill()
    {
        

        if (OnCooldown())
        {
            Debug.Log("On Cooldown");
            return false;
        }
        return true;
    }

   
    public void SetSkill(SkillDataSO skillData)
    {

     
        player.ui.inGameUI.GetNewSkillSlot().SetupSkillSlot(skillData);
        
        int skillsSize= skillManager.playerSkillSlots.Length;
       for(int i = 0;i<skillsSize; i++)
        {
            if (skillManager.playerSkillSlots[i] == SkillType.None )
            {
                skillManager.playerSkillSlots[i] = skillData.skillData.skillType;
                return;
            }

        }
        ResetCooldown();
    }
    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown()
    {

        player.ui.inGameUI.GetSkillSlot(skillType).StartCooldown(cooldown);
        lastTimeUsed = Time.time;

    }

    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;

    public void ResetCooldown()
    {
        player.ui.inGameUI.GetSkillSlot(skillType).ResetCooldown();
        lastTimeUsed = Time.time - cooldown;

    }


}
