using UnityEngine;

public class Skill_Windblade : SkillBase
{
    public GameObject windBladePrefab;
    private SkillObject_Windblade SkillObject_Windblade;


    public override void TryUseSkill()
    {
        base.TryUseSkill();
       
    }

    public void CreaterWindBlade()
    {
        GameObject windBlade = Instantiate(windBladePrefab, transform.position, Quaternion.identity);
        SkillObject_Windblade = windBlade.GetComponent<SkillObject_Windblade>();
        SkillObject_Windblade.Setup(this);


    }


}
