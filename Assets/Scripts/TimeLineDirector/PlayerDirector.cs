using UnityEngine;
public enum playerDirectorState
{
    idle,move,one
}
public class PlayerDirector : MonoBehaviour
{
    public static PlayerDirector instance;
    private Player player;
    public StateMachine stateMachine;
    public playerDirectorState nowState;
    public SpriteRenderer sr;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

        player = Player.instance;
        
        stateMachine = player.stateMachine;
        if (stateMachine == null)
        {
            Debug.Log("StateMachine is null");
        }
    }

    public void SetIdleState()
    {
        if (player == null) player = Player.instance;
        nowState = playerDirectorState.idle;
        //stateMachine.ChangeState(player.idleState);
    }

    public void SetMoveState()
    {
        if (player == null) player = Player.instance;
        nowState = playerDirectorState.move;
        //stateMachine.ChangeState(player.moveState);
    }

    public void Flip()
    {
        if (player == null) player = Player.instance;

        sr = player.sr;
        //Debug.Log("Filp");
        sr.flipX = !sr.flipX;
    }


}
