using UnityEngine;

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
        bool seeSmthn=IsCanSee.PerformRaycast
        (
            agent._weapon.aimPos.position,
            agent._weapon.aimPos.forward,
            agent.detectionRadius/2,
            agent.enemyLayer
        );
        if (agent.playerTransform!=null
            &&(agent.playerTransform.position-agent.transform.position).magnitude <= agent.navMeshAgent.stoppingDistance
            &&seeSmthn)
        {
            AiHealt enemyHealt = agent.playerTransform.GetComponentInParent<AiHealt>();
            agent._weapon.CallFire();

            if (enemyHealt != null && enemyHealt.isDead)
            {
                agent.playerTransform = null;
            }
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
