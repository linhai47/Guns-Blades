using UnityEngine;

public class Skill_EyeofCyclone :SkillBase
{
    public GameObject EyeOfCyclonePrefab;

    private SkillObject_EyeofCyclone SkillObject_EyeOfCyclone;


    public override void TryUseSkill()
    {

        base.TryUseSkill();
        

    }

    public void CreateEyeOfCyclone()
    {
        GameObject EyeOfCyclone = Instantiate(EyeOfCyclonePrefab, transform.position, Quaternion.identity);
        SkillObject_EyeOfCyclone = EyeOfCyclone.GetComponent<SkillObject_EyeofCyclone>();
        SkillObject_EyeOfCyclone.Setup(this ,false);

    }

}
