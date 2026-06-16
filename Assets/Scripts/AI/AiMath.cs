using UnityEngine;

public static class AiMath
{
    
    public static bool IsTargetInRange(this Transform source, Transform target, float range)
    {
        if (source == null || target == null) return false;

        Vector3 direction = target.position - source.position;
        
        float squareDistance = direction.sqrMagnitude;
        float squareRange = range * range;

        return squareDistance < squareRange;
    }
}