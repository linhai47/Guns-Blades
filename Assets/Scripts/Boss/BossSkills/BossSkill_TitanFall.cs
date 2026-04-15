using UnityEngine;

public class BossSkill_TitanFall : MonoBehaviour
{
    public Boss boss;
    public float jumpScale = 2f;
    public float gravityScale = 2f;

    private void Start()
    {
        boss = GetComponentInParent<Boss>();
    }


    public void TryUseSkill()
    {
        

        boss.rb.gravityScale *= gravityScale;
        boss.SetVelocity(boss.rb.linearVelocity.x, boss.jumpForce * jumpScale);

        
    }

    public void BackToNormal()
    {


        boss.rb.gravityScale /= gravityScale;
   
     
    }
}
