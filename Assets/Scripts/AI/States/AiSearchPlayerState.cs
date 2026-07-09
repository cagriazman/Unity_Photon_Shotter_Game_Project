using Photon.Pun;
using UnityEngine;
public class AiSearchPlayerState : AiState
{
        public AiStateId GetId()
        {
            return AiStateId.SearchPlayer;
        }

        public void Enter(AiAgent agent)
        {
            
        }

        public void Update(AiAgent agent)
        {
            int index = agent.pwId % 4;
            if(agent.playerTransform != null)agent.stateMachine.ChangeState(AiStateId.ChasePlayer);

            //patrol
            agent.currentSpeed = agent.navMeshAgent.velocity.magnitude;
            agent.navMeshAgent.SetDestination(agent.patrolPoints[3]);
            agent.animator.SetFloat("Ai_dikey_yon", agent.currentSpeed, 0.2f, Time.deltaTime);
            
            if(!PhotonNetwork.IsMasterClient)return;
            if (agent.playerTransform == null && agent.Sensor.colliders != null && agent.Sensor.colliders.Length > 0)
            {

                
                for (int i = 0; i < agent.Sensor.colliders.Length; i++)
                { 
                    if (agent.Sensor.colliders[i] == null) 
                    {
                        continue; 
                    }
                    GameObject playerObj = agent.Sensor.colliders[i].gameObject;

                    if (ValidateTeam.PerformTeam(agent, playerObj))
                    {
                        continue;
                    }
                    
                    
                    
                    agent.changeId(FindPwId.GetViewId(playerObj)); 
                    break;
                    
                    
                }

                

                
            }
        }

        //private void ValidateTeam(AiAgent agent,GameObject playerObj)
        // {
        //     OyuncuIcın player = playerObj.GetComponentInParent<OyuncuIcın>();
        //     AiAgent bot = playerObj.GetComponentInParent<AiAgent>();
        //     AiHealt botHealt = playerObj.GetComponentInParent<AiHealt>();
        //
        //     int TempTeamNumber = -1; 
        //
        //     if (player != null)
        //     {
        //         TempTeamNumber = player.teamNumber;
        //     }
        //     else if (bot != null)
        //     {
        //         TempTeamNumber = bot.teamNumber;
        //         if (botHealt.isDead)
        //         {
        //             continue;
        //         }
        //     }
        //     else
        //     {
        //         continue; 
        //     }
        //     if (agent.teamNumber == TempTeamNumber || TempTeamNumber == -1)
        //     {
        //         continue;
        //     }
        //     
        // }

        public void Exit(AiAgent agent)
        {
            
        }
}
