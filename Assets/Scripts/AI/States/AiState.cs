using UnityEngine;


    public enum AiStateId
    {
        ChasePlayer,
        SearchPlayer,
        AttackPlayer
        
    }

    public interface AiState
    {
        
        AiStateId GetId();
        void Enter(AiAgent agent);
        void Update(AiAgent agent);
        void Exit(AiAgent agent);
    }
    

