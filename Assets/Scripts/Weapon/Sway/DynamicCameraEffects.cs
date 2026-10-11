using UnityEngine;
using Unity.Cinemachine;
using UnityEditor.U2D.Sprites;

public class DynamicCinemachineEffects : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("FOV Settings")]
    [SerializeField] private float defaultFOV = 40f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovTransitionSpeed = 5f;
    [SerializeField] private float maxSpeedLimit = 7f;

    [Header("Movement Tilt & Shift Settings")]
    [Tooltip("Sağa/sola (Strafing) giderken kameranın yatış (Roll/Eğilme) açısı.")]
    [SerializeField] private float maxLeanAngle = 2f; 
    [Tooltip("Sağa/sola giderken kameranın yanal (X) kayma miktarı.")]
    [SerializeField] private float maxSideShift = 0.15f; 
    [Tooltip("Yeniden merkezlenme ve esneme hızı.")]
    [SerializeField] private float tiltTransitionSpeed = 4f;

    private float currentFOV;
    private float currentRoll;
    private float currentXShift;
    private Vector3 originalCameraOffset;
    private CinemachineTransposer transposer;

    void Start()
    {
        if (playerController == null)
            playerController = GetComponentInParent<CharacterController>();

        if (virtualCamera == null)
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            currentFOV = virtualCamera.m_Lens.FieldOfView;
            currentRoll = virtualCamera.m_Lens.Dutch; // Dutch, Cinemachine'de Z-tilt (Roll) açısıdır
            
            // Eğer kamera transposer kullanıyorsa lokal offset'i almak için
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                originalCameraOffset = transposer.m_FollowOffset;
            }
        }
    }

    void Update()
    {
        if (playerController == null || virtualCamera == null) return;
        
        Vector3 localVelocity = playerController.transform.InverseTransformDirection(playerController.velocity);
        
        float forwardSpeed = localVelocity.z; 
        float lateralSpeed = localVelocity.x; 

        Vector3 horizontalVelocity = new Vector3(playerController.velocity.x, 0, playerController.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeedLimit);

        if (speedRatio > 0.01f)
        {
            
        }
        float targetFOV = Mathf.Lerp(defaultFOV, sprintFOV, speedRatio);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovTransitionSpeed);
        virtualCamera.m_Lens.FieldOfView = currentFOV;

        // ==========================================
        // 3. Sağa / Sola Eğilme (Tilt / Lean) & Kayma (Shift)
        // ==========================================
        // Sağa giderken (lateralSpeed > 0) kamera sola yatmalı (-maxLeanAngle)
        // Sola giderken (lateralSpeed < 0) kamera sağa yatmalı (+maxLeanAngle)
        float targetRoll = -(lateralSpeed / maxSpeedLimit) * maxLeanAngle;
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * tiltTransitionSpeed);
        virtualCamera.m_Lens.Dutch = currentRoll; 

        // Sağa/Sola giderken kamerayı hafifçe o yönün aksine kaydırarak atalet hissi verelim (Transposer offset'i ile)
        if (transposer != null)
        {
            // Sağa giderken sola kayma, sola giderken sağa kayma
            float targetXShift = -(lateralSpeed / maxSpeedLimit) * maxSideShift;
            currentXShift = Mathf.Lerp(currentXShift, targetXShift, Time.deltaTime * tiltTransitionSpeed);

            // Geriye giderken de kameranın hafifçe öne (veya arkaya) esnemesi
            float targetZShift = (forwardSpeed < 0) ? 0.1f : 0f; // Geri giderken hafif öne yığılma

            transposer.m_FollowOffset = originalCameraOffset + new Vector3(currentXShift, 0, targetZShift);
        }
    }
}