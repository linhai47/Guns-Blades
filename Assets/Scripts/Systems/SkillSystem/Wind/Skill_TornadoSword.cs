using UnityEngine;

public class Skill_TornadoSword : SkillBase
{
    public GameObject TornadoSwordPrefab;

    private SkillObject_TornadoSword SkillObject_TornadoSword;


    public override void TryUseSkill()
    {

        base.TryUseSkill();


    }

    public void CreateTornado()
    {
        GameObject tornado = Instantiate(TornadoSwordPrefab, transform.position, Quaternion.identity);
        SkillObject_TornadoSword = tornado.GetComponent<SkillObject_TornadoSword>();
        SkillObject_TornadoSword.Setup(this);

    }


}
