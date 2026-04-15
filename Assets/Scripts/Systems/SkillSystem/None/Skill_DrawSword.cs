using UnityEngine;

public class Skill_DrawSword : SkillBase
{
    public GameObject DrawSwordPrefab;

    private SkillObject_DrawSword SkillObject_DrawSword;

    public override void TryUseSkill()
    {
        base.TryUseSkill();
    }


    public void CreateDrawSword()
    {
        GameObject DrawSword = Instantiate(DrawSwordPrefab, transform.position, Quaternion.identity);
        SkillObject_DrawSword = DrawSword.GetComponent<SkillObject_DrawSword>();
        SkillObject_DrawSword.Setup(this);

    }
}
