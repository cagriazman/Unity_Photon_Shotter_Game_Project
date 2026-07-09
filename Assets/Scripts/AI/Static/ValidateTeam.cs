using UnityEngine;

public static class ValidateTeam
{
    public static bool PerformTeam(AiAgent agent,GameObject playerObj)
    {
        OyuncuIcın player = playerObj.GetComponentInParent<OyuncuIcın>();
        AiAgent bot = playerObj.GetComponentInParent<AiAgent>();
        AiHealt botHealt = playerObj.GetComponentInParent<AiHealt>();
        
        int TempTeamNumber = -1; 
        
        if (player != null)
        {
            TempTeamNumber = player.teamNumber;
            
        }
        else if (bot != null)
        {
            TempTeamNumber = bot.teamNumber;
            if (botHealt.isDead)
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        if (agent.teamNumber == TempTeamNumber || TempTeamNumber == -1)
        {
            return true;
        }

        return false;

    }
    
}
