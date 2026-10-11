using UnityEngine;
using Unity.Cinemachine; 

public class DynamicCinemachineFOV : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera; 

    [Header("FOV Settings")]
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovTransitionSpeed = 5f;
    [SerializeField] private float maxSpeedLimit = 7f;

    private float currentFOV;

    void Start()
    {
        if (playerController == null)
            playerController = GetComponentInParent<CharacterController>();

        if (virtualCamera == null)
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        if (virtualCamera != null)
            currentFOV = virtualCamera.m_Lens.FieldOfView;
        else
            currentFOV = defaultFOV;
    }

    void Update()
    {
        if (playerController == null || virtualCamera == null) return;

        Vector3 horizontalVelocity = new Vector3(playerController.velocity.x, 0, playerController.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeedLimit);

        float targetFOV = Mathf.Lerp(defaultFOV, sprintFOV, speedRatio);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovTransitionSpeed);

        virtualCamera.m_Lens.FieldOfView = currentFOV;
    }
}