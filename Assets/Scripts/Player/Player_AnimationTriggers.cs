using Unity.VisualScripting;
using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;
    [Header ("Wind")]
    private Skill_Windblade skill_Windblade;
    private Skill_TornadoSword skill_TornadoSword;
    private Skill_EyeofCyclone skill_EyeofCyclone;
    private Skill_WindPulls skill_WindPulls;
    private Skill_StormRools skill_StormRools;
    [Header ("None")]
    private Skill_DrawSword skill_DrawSword;



    [Header("Fire")]
    private Skill_ShadowlessFist skill_ShadowlessFist;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        skill_Windblade =player.GetComponentInChildren<Skill_Windblade>();
        skill_TornadoSword = player.GetComponentInChildren<Skill_TornadoSword>();
        skill_EyeofCyclone = player.GetComponentInChildren<Skill_EyeofCyclone>();
        skill_WindPulls = player.GetComponentInChildren<Skill_WindPulls>();
        skill_StormRools = player.GetComponentInChildren<Skill_StormRools>();

        skill_DrawSword = player.GetComponentInChildren <Skill_DrawSword>();

        skill_ShadowlessFist = player.GetComponentInChildren<Skill_ShadowlessFist>();
    }
    private void Skill_WindbladeTrigger()
    {
        skill_Windblade.CreaterWindBlade();
        animationOver =true;
    }

    private void Skill_TornadoSwordTrigger()
    {
        skill_TornadoSword.CreateTornado();
        animationOver=true;

    }

    private void Skill_EyeofCycloneTrigger()
    {
        skill_EyeofCyclone.CreateEyeOfCyclone();
        animationOver = true;

    }
    private void Skill_WindPullTrigger()
    {
        skill_WindPulls.CreateWindPull();
        animationOver=true;
    }

    private void Skill_StormRoolsTrigger()
    {
        skill_StormRools.CreateStormRools();
        animationOver=true;
    }

    private void Skill_DrawSwordTrigger()
    {
        skill_DrawSword.CreateDrawSword();
        animationOver=true;
    }

    private void Skill_ShadowlessFistTrigger()
    {
        skill_ShadowlessFist.AttackTarget();
        animationOver=true;
    }

    private void Skill_LockOnTrigger()
    {
        skill_ShadowlessFist.LockOnTarget();
        animationOver = true;
    }
}
