using System.Collections.Generic;
using UnityEngine;


public class AiMemory
{
    public GameObject gameObject;
    public float distance;
    public Vector3 direction;
    public Vector3 position;
    public float angle;
    public float seenTime;
    

}
public class AiSensoryMemory
{
    
    public List<AiMemory> memories = new List<AiMemory>();
    
    

    public void UpdateMemories(AiSensor sensor)
    {
        int count =sensor.Objects.Count;
        
        for (int i = 0; i < count; ++i)
        {
            GameObject target = sensor.Objects[i];
            RefreshingMemory(sensor.gameObject,target);
        }
        
    }

    public AiMemory IsFirstTarget(float minDistance)
    {
        AiMemory memory = memories.Find(x =>x.distance == minDistance);
        if (memory != null)
        {

            return memory;
        }
        return null;
    }
    
    

    void RefreshingMemory(GameObject agent,GameObject target)
    {
        if (target == null || agent == null)
        {
            return; 
        }
        AiMemory memory = FetchMemory(target);
        memory.gameObject = target;
        memory.position = target.transform.position;
        memory.direction = agent.transform.position - target.transform.position; 
        memory.angle = Vector3.Angle(memory.direction, agent.transform.forward);
        memory.distance = memory.direction.magnitude;
        memory.seenTime = Time.time;
        
        
    }

    AiMemory FetchMemory(GameObject gameObject)
    {
        AiMemory memory = memories.Find(x =>x.gameObject == gameObject);
        if (memory == null)
        {
            memory = new AiMemory();
            memories.Add(memory);
        }
        return memory;
    }
    
    public void RemoveMemory(float olderThan)
    {
        float currentTime = Time.time;
        
        memories.RemoveAll(x =>x.seenTime + olderThan < currentTime);
        memories.RemoveAll(x =>!x.gameObject);
        memories.RemoveAll(x => x.gameObject.GetComponent<HealtBase>()?.isDead == true);
    }

    
    
    
}
