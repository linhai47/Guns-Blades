using UnityEngine;

public class Player_CastingState : PlayerState
{
    
    private Player_AnimationTriggers animationTriggers;


    private float gravity;
    public Player_CastingState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public string triggerName;
    
    

    public override void Enter()
    {
        base.Enter();
        if(triggerName != null) 
        anim.SetTrigger(triggerName);
        stateTimer = GetDuration(triggerName );
        gravity = rb.gravityScale;
        rb.gravityScale = 0f;
    }




    public override void Update()
    {
        base.Update();
        player.rb.linearVelocity = Vector2.zero; 
        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = gravity;
    }
    public void SetTrigger(string triggerName)
    {
        this.triggerName = triggerName;

    }
    public float GetDuration( string triggerName)// 实在没有好的方法，手动管理吧
    {

        Animator animator = player.anim;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
  
        // 遍历所有 AnimationClip
        foreach (AnimationClip clip in ac.animationClips)
        {
         
            if (triggerName == "CastSlash") // 动画片段名
            {
                
                return .2f;
            }
            if (triggerName == "CastUp")
            {
             
                return .2f;
            }
            if(triggerName == "CastBlast")
            {
               
                return .5f;
            }
            if(triggerName == "CastHold")
            {
              
                return .8f;
            }
            if(triggerName == "CastDrawSword")
            {
                return .7f;
            }

            if(triggerName == "CastStormRools")
            {
                return .8f;
            }
            if(triggerName == "CastShadowlessFist")
            {
                return 1.5f;
            }

        }
        return 0f;
    }

    //private static float getDuration(AnimationClip clip)
    //{
    //    float clipLength = clip.length; // 秒
    //    Debug.Log(clipLength);
    //    return (float)clipLength;
    //}
}
