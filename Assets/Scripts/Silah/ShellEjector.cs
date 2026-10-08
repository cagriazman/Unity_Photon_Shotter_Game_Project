using UnityEngine;

public class ShellEjector : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject shellPrefab; 
    [SerializeField] private Transform ejectPoint;   

    [Header("Kovan Fırlatma Gücü")]
    [SerializeField] private float minForce = 3.0f;
    [SerializeField] private float maxForce = 5.0f;

    [Header("Dağılma / Sapma Açısı (Spread)")]
    [SerializeField] private float spreadAngle = 15.0f;

    [Header("Dönme (Torque) Miktarı")]
    [SerializeField] private float torqueForce = 20.0f;

    [Header("Temizlik")]
    [SerializeField] private float destroyDelay = 5.0f; 

   
    public void EjectShell()
    {
        if (shellPrefab == null || ejectPoint == null) return;

        GameObject shell = Instantiate(shellPrefab, ejectPoint.position, ejectPoint.rotation);

        Rigidbody rb = shell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 baseDirection = (ejectPoint.right * 0.8f + ejectPoint.up * 0.4f).normalized;

            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle)
            );
            Vector3 finalDirection = spreadRotation * baseDirection;

            float randomSpeed = Random.Range(minForce, maxForce);
            rb.linearVelocity = finalDirection * randomSpeed;

            Vector3 randomTorque = new Vector3(
                Random.Range(-torqueForce, torqueForce),
                Random.Range(-torqueForce, torqueForce),
                Random.Range(-torqueForce, torqueForce)
            );
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }

        Destroy(shell, destroyDelay);
    }
}