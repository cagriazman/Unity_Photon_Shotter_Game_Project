using System;
using UnityEngine;

[ExecuteInEditMode]
public class AiTargetingSystem : MonoBehaviour
{
    AiSensoryMemory MemorySystem = new AiSensoryMemory();
    AiSensor sensor;
    private AiMemory BestMemory;
    [SerializeField]private float timeSpan = 3f;

    public bool HasTarget => BestMemory != null; 
    public float TargetDistance => BestMemory.distance;
    public Vector3 TargetPosition => BestMemory.position;
    
    
    private void Start()
    {
        sensor = GetComponent<AiSensor>();
    }

    private void Update()
    {
        MemorySystem.UpdateMemories(sensor);
        MemorySystem.RemoveMemory(timeSpan);
        BestMemory = MemorySystem.IsFirstTarget(MinDistanceEvaluation());
    }
    float MinDistanceEvaluation()
    { 
        float minDistance = 20f;
        foreach (var memory in MemorySystem.memories)
        {
            if(minDistance>memory.distance)
            {
                minDistance = memory.distance;
            }
        }

        return minDistance;
    }
    void OnDrawGizmos()
    {
        foreach (var memory in MemorySystem.memories)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(memory.position, 0.2f);
            
        }
        if(BestMemory !=null)
        {
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(BestMemory.position, 0.3f);
        }
    }
}
