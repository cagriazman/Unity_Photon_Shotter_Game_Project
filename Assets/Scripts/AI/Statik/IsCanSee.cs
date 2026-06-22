using UnityEngine;


public static class IsCanSee
{
    
    public static bool PerformRaycast(Vector3 origin, Vector3 direction, float distance, LayerMask targetMask)
    {
        
        return Physics.Raycast(origin, direction, distance, targetMask);
    }

    
}