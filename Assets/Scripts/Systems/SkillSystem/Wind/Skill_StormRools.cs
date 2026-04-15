using UnityEngine;

public class Skill_StormRools : SkillBase
{
    public GameObject StormRoolsPrefab;

    private SkillObject_StormRools SkillObject_StormRools;

    public override void TryUseSkill()
    {
        base.TryUseSkill();
    }


    public void CreateStormRools()
    {
        GameObject StormRools = Instantiate(StormRoolsPrefab, transform.position, Quaternion.identity);
        SkillObject_StormRools = StormRools.GetComponent<SkillObject_StormRools>();
        SkillObject_StormRools.Setup(this);

    }
}
