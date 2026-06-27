using UnityEngine;
using UnityEngine.InputSystem.Android;

public class AiAttackPlayerState : AiState
{ 
    public AiStateId GetId()
    {
        return AiStateId.AttackPlayer;
    }

    public void Enter(AiAgent agent)
    {
        
        
    }

    public void Update(AiAgent agent)
    {
        
        if (agent.playerTransform!=null
            &&(agent.playerTransform.position-agent.transform.position).magnitude <= agent.navMeshAgent.stoppingDistance
            &&agent.seeSmthn)
        {
            
        }
        else
        {
            agent.stateMachine.ChangeState(AiStateId.SearchPlayer);

        }
    }
   

    public void Exit(AiAgent agent)
    {
    }
    
}
