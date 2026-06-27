using Photon.Pun;
using UnityEngine;


public class AiChasePlayerState : AiState
{
    bool lockedIn = true;
    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance=13f; 
        lockedIn = true;
        
    }

    public void Update(AiAgent agent)
    {
        float currentAnimSpeedValue=agent.animator.GetFloat("Ai_dikey_yon");
        if (agent.playerTransform != null)
        {
            ///////////////////////////////////////////////////////////////////////////
            if (agent.transform.IsTargetInRange(agent.playerTransform, agent.detectionRadius))
            {
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
                    
                
                 
                
                if ((targetDistance - agent.navMeshAgent.stoppingDistance)<0.01f )
                {
                    if (agent.seeSmthn)
                    {
                        agent.navMeshAgent.ResetPath();
                        AiHealt enemyHealt = agent.playerTransform.GetComponentInParent<AiHealt>();
                        agent._weapon.CallFire();

                        if (enemyHealt != null && enemyHealt.isDead)
                        {
                            agent.playerTransform = null;
                        }
                    }

                    else
                    {
                        lockedIn = false;

                            Vector3 dirToAi = (agent.playerTransform.position - agent.transform.position).normalized;
                            // float signedAngle = Vector3.SignedAngle(agent.transform.forward, dirToAi, Vector3.up);
                            // float leftOrRight = Mathf.Sign(signedAngle);
                            //float flankAngle = leftOrRight * 60f; 
                            //Vector3 rotatedDir = Quaternion.Euler(0, flankAngle, 0) * dirToAi;
                            
                            
                            Vector3 newFlankPosition = ( dirToAi * 8f);

                            agent.navMeshAgent.SetDestination(agent.playerTransform.position + newFlankPosition);
                            
                            
                            
                            // Debug.DrawLine(agent.transform.position, agent.transform.position+dirToAi, Color.blue, 10);
                            // Debug.DrawLine(agent.transform.position, agent.transform.position+rotatedDir, Color.yellow, 0.1f);
                            // Debug.DrawLine(agent.transform.position, agent.transform.position+newFlankPosition, Color.white, 0.1f);
    
                            Debug.Log("Seni duvardan göremiyorum, sağına doğru açılıyorum!");
                        
                        
                    }
                }
                else
                {
                    lockedIn = true;
                    
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