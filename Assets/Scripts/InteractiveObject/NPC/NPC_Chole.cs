using UnityEngine;

public class NPC_Chole : Object_NPC
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void Enter()
    {
        base.Enter();
        currentState = NPCState.Idle;
    }
    protected override void ChangeIdle()
    {
        base.ChangeIdle();


    }
    protected override void ChangePatrol()
    {
        base.ChangePatrol();


    }
    protected override void ChangeTalk()
    {
        base.ChangeTalk();
        rb.linearVelocity = Vector2.zero;
    }

    protected override void ChangeTaskOver()
    {
        base.ChangeTaskOver();
    }


}