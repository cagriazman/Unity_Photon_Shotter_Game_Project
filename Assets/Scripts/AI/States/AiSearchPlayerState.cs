using System.Linq;
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

            agent.currentSpeed = agent.navMeshAgent.velocity.magnitude;
            agent.navMeshAgent.SetDestination(agent.patrolPoints[3]);
            agent.animator.SetFloat("Ai_dikey_yon", agent.currentSpeed, 0.2f, Time.deltaTime);
            
            if(!PhotonNetwork.IsMasterClient)return;
            if (agent.playerTransform == null)
            {
                GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
                GameObject[] botObjects = GameObject.FindGameObjectsWithTag("Ai");
                GameObject[] lastObjects = playerObjects.Concat(botObjects).ToArray();
                
                foreach (GameObject playerObj in lastObjects)
                {                       
                    OyuncuIcın player = playerObj.GetComponentInParent<OyuncuIcın>();
                    AiAgent bot = playerObj.GetComponentInParent<AiAgent>();
                    AiHealt botHealt = playerObj.GetComponentInParent<AiHealt>();

                    int hedefTakimNumarasi = -1; 

                    if (player != null)
                    {
                        hedefTakimNumarasi = player.teamNumber;
                    }
                    else if (bot != null)
                    {
                        hedefTakimNumarasi = bot.teamNumber;
                        if (botHealt.isDead)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue; 
                    }
                    if (agent.teamNumber == hedefTakimNumarasi || hedefTakimNumarasi == -1)
                    {
                        continue;
                    }
                    
                    if (agent.transform.IsTargetInRange(playerObj.transform, agent.detectionRadius))
                    {
                        PhotonView pwNewObject = playerObj.GetComponentInParent<PhotonView>();
                        if (pwNewObject != null )
                        {
                            int id = pwNewObject.ViewID;

                            agent.changeId(id);

                            break;
                            
                        }
                    }
                    
                }

                

                
            }
        }

        public void Exit(AiAgent agent)
        {
            
        }
}
