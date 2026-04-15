using UnityEngine;

public class Skill_WindPulls : SkillBase
{
    public GameObject windPullPrefab;

   private SkillObject_WindPulls skillObject_windPulls;

    public override void TryUseSkill()
    {
        base.TryUseSkill();
    }


    public void CreateWindPull()
    {
        GameObject windPull= Instantiate(windPullPrefab, transform.position, Quaternion.identity);
        skillObject_windPulls = windPull.GetComponent<SkillObject_WindPulls>();
        skillObject_windPulls.Setup(this);

    }
}
