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
    [SerializeField] private float speedRatioLimit = 5.701f;

    [Header("Movement Tilt & Shift Settings")]
    [Tooltip("Sağa/sola (Strafing) giderken kameranın yatış (Roll/Eğilme) açısı.")]
    [SerializeField] private float maxLeanAngle = 2f; 
    
    [Tooltip("Yeniden merkezlenme ve esneme hızı.")]
    [SerializeField] private float tiltTransitionSpeed = 4f;

    [SerializeField] private float currentFOV;
    
    [SerializeField] private float currentRoll;
    
    
    [Header("Roll / Lean Settings")]
    [SerializeField] private float maxMouseLeanAngle = 1f;     
    [SerializeField] private float totalMaxRoll = 4f;          
    [SerializeField] private float leanSmoothSpeed = 10f;      


    

    void Start()
    {
        if (playerController == null)
            playerController = GetComponentInParent<CharacterController>();

        if (virtualCamera == null)
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            currentFOV = virtualCamera.m_Lens.FieldOfView;
            currentRoll = virtualCamera.m_Lens.Dutch; 
            
           
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
        
        float speedRatio;
        
        if (currentSpeed > speedRatioLimit)
        {
             speedRatio = Mathf.Clamp01(currentSpeed / maxSpeedLimit);

        }
        else  
        {
             speedRatio = Mathf.Clamp01(currentSpeed  / maxSpeedLimit);
             speedRatio /= 2.5f;
        }

        float targetFOV = Mathf.Lerp(defaultFOV, sprintFOV, speedRatio);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovTransitionSpeed);
        virtualCamera.m_Lens.FieldOfView = currentFOV;

        float movementRoll = -(lateralSpeed / maxSpeedLimit) * maxLeanAngle;


        float mouseX = Input.GetAxisRaw("Mouse X");
        Debug.Log(mouseX);
        if (Mathf.Abs(mouseX) > 1.2f)
        {
            maxMouseLeanAngle = 2f;
        }
        else
        {
            maxMouseLeanAngle = 1;
        }
        float mouseRoll = -mouseX * maxMouseLeanAngle;

        float targetRoll = Mathf.Clamp(movementRoll + mouseRoll, -totalMaxRoll, totalMaxRoll);

        
        //float targetRoll = -(lateralSpeed / maxSpeedLimit) * maxLeanAngle;
        
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * tiltTransitionSpeed);
        virtualCamera.m_Lens.Dutch = currentRoll; 

        
    }
}