using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class EntityState
{
    protected string animBoolName;
    protected StateMachine stateMachine;

    protected Animator anim;
    protected Rigidbody2D rb;
  
    protected float stateTimer;

    protected bool triggerCalled;
    protected Entity_Stats stats;

    public EntityState(StateMachine stateMachine , string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

    }

    public virtual void Enter()
    {

        anim.SetBool(animBoolName, true);
        triggerCalled = false;

    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        UpdateAnimationParameters();
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
       
    }

    public void AnimationTrigger()
    {

        triggerCalled = true;
    }
    public virtual void UpdateAnimationParameters()
    {

    }
    public void SyncAttackSpeed()
    {
        if(stats == null)
        {
            Debug.Log("Stats is null");
        }
        float attackSpeed = stats.offense.attackSpeed.GetValue();
        anim.SetFloat("attackSpeedMultiplier", attackSpeed);
    }

}
