using Photon.Pun;
using UnityEngine;


public class AiChasePlayerState : AiState
{
    private float counter = 1;
    bool lockedIn = true;
    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
        lockedIn = true;
        
    }

    public void Update(AiAgent agent)
    {
        float currentAnimSpeedValue=agent.animator.GetFloat("Ai_dikey_yon");
        if (agent.playerTransform != null)
        {
            if (agent.transform.IsTargetInRange(agent.playerTransform, agent.detectionRadius))
            {
                ///////////////////////////////////////////////////////////////////////////NavMeshAgent Path Visual
                UnityEngine.AI.NavMeshPath currentPath = agent.navMeshAgent.path;
                if (currentPath.corners.Length > 1)
                {
                    for (int i = 0; i < currentPath.corners.Length - 1; i++)
                    {
                        Debug.DrawLine(currentPath.corners[i], currentPath.corners[i + 1], Color.black, 0.1f);
                        Debug.DrawRay(currentPath.corners[i], Vector3.up * 0.5f, Color.yellow, 0.1f);
                    }
                }
                ///////////////////////////////////////////////////////////////////////////
                
                
                agent.currentSpeed = agent.navMeshAgent.velocity.magnitude;

                if(lockedIn)agent.navMeshAgent.SetDestination(agent.playerTransform.position);
                agent.animator.SetFloat("Ai_dikey_yon", agent.currentSpeed, 0.2f, Time.deltaTime);

                float targetDistance = (agent.playerTransform.position - agent.transform.position).magnitude;
                    
                
                if (agent.aiTargetingSystem.HasTarget)
                {
                        //lockedIn = false;
                        agent.navMeshAgent.ResetPath();
                        //counter = 1f;
                        AiHealt enemyHealt = agent.playerTransform.GetComponentInParent<AiHealt>();
                        
                        agent._weapon.CallFire();
                        if (enemyHealt != null && enemyHealt.isDead)
                        {
                            agent.playerTransform = null;
                        }
                }

                
                
                
               
            }
            else
            {
                //agent.animator.SetFloat("Ai_dikey_yon", 0f, 0.25f, Time.deltaTime);
                 
                
                    if(PhotonNetwork.IsMasterClient) agent.changeId(0);
                
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