using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AiAgent : MonoBehaviourPun
{
    public float currentSpeed;
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public Transform playerTransform ;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public float detectionRadius = 10.0f;
    public bool isEnemyDead;
    public Vector3[] patrolPoints;

    public int pwId;
    public LayerMask enemyLayer;

    public bool onOffGizmos;
    
    [FormerlySerializedAs("pwId")] public int cacheId;
    public aiWeapon _weapon;
    
    [Header("Takım Materyal Paketleri")] public Material[] saldiriPaketi;  
    public Material[] savunmaPaketi;
   public SkinnedMeshRenderer meshRenderer;
    public GameObject tpOyuncuGorunum;
    public int teamNumber =-1;

    void Start()
    {
        patrolPoints = new Vector3[]
        { 
            new Vector3(-35.9799995f,1.17400002f,-31.3899994f),
            new Vector3(-13.081521f,3.37899995f,-63.8100014f),
            new Vector3(15.3590002f,-3.97000003f,-66.2009964f),
            new Vector3(58f,0.0799999982f,-61.4799995f)
        };
        pwId = photonView.ViewID;
        
        
        object[] toRegards = photonView.InstantiationData;
        _weapon = GetComponentInChildren<aiWeapon>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiSearchPlayerState());
        stateMachine.RegisterState(new AiAttackPlayerState());
        stateMachine.ChangeState(initialState);
         teamNumber= (int)toRegards[0];
       
        
        GorseliUygula(teamNumber);
    }

    
    void Update()
    {
        stateMachine.Update();
        Debug.Log(stateMachine.currentState);

        
    }
    public void changeId(int newId)
    {
        if (cacheId != newId)
        {
            photonView.RPC("syncId", RpcTarget.AllBuffered, newId);
        }
    }
    
    [PunRPC]
    public void syncId(int newId)
    {
        cacheId = newId;
        if (newId == 0) playerTransform = null;
        PhotonView hedefinPW = PhotonView.Find(newId);
        
        if (hedefinPW != null)
        {
            playerTransform = hedefinPW.transform;
        }


    }

    


    void GorseliUygula(int takimNo)
    {
        if (meshRenderer == null) return;
        if (takimNo == 0)
        {
            meshRenderer.sharedMaterials = saldiriPaketi; 
        }
        else if (takimNo == 1)
        {
            meshRenderer.sharedMaterials = savunmaPaketi;
        }
    }
    private void OnDrawGizmos()
    {
        while(onOffGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position,detectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, navMeshAgent.stoppingDistance);
            
        }
        
        // Gizmos.color = Color.blue;
        // Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}

