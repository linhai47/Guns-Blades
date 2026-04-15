using UnityEngine;

public class NPC_Myra : Object_NPC
{
    protected override void Awake()
    {
        base.Awake();
    }



    protected override void Enter()
    {
        base.Enter();
     
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