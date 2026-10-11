using UnityEngine;
using UnityEngine.VFX;

public class PersistentTracer : MonoBehaviour
{
    [SerializeField] private VisualEffect tracerPrefab;
    [SerializeField] private float tracerDuration = 1.0f; 

    public void SpawnTracer(Vector3 hitPoint)
    {
        if (tracerPrefab == null ) return;

        VisualEffect tracer = Instantiate(tracerPrefab, hitPoint, Quaternion.identity);
        
        
        tracer.Play();
        Destroy(tracer.gameObject, tracerDuration);
    }
}