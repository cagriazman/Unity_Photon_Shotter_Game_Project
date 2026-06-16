using Photon.Pun;
using UnityEngine;


public class AiChasePlayerState : AiState
{
    private float timer = 0;

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
        
    }

    public void Update(AiAgent agent)
    {
        float currentAnimSpeedValue=agent.animator.GetFloat("Ai_dikey_yon");
        if (agent.playerTransform != null)
        {
            if (agent.transform.IsTargetInRange(agent.playerTransform, agent.detectionRadius))
            {
                agent.currentSpeed = agent.navMeshAgent.velocity.magnitude;

                agent.navMeshAgent.SetDestination(agent.playerTransform.position);
                agent.animator.SetFloat("Ai_dikey_yon", agent.currentSpeed, 0.2f, Time.deltaTime);
                
                if (((agent.playerTransform.position-agent.transform.position).magnitude - agent.navMeshAgent.stoppingDistance)<0.01f&&currentAnimSpeedValue<=0.01f)
                {
                    agent.stateMachine.ChangeState(AiStateId.AttackPlayer);
                }
            }
            else
            {
                agent.animator.SetFloat("Ai_dikey_yon", 0f, 0.25f, Time.deltaTime);
                 
                if (currentAnimSpeedValue < 0.01f)
                {
                    if(PhotonNetwork.IsMasterClient) agent.changeId(0);
                }
            }
        }
        else
        {
            agent.stateMachine.ChangeState(AiStateId.SearchPlayer);
        }
    }
    

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.ResetPath(); 

    }
}