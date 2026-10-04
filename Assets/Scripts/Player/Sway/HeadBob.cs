using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkingSpeed = 14f;       
    public float bobAmountY = 0.025f;       
    public float bobAmountX = 0.02f;       

    [Header("Sprint (Koşma) Ayarları")]
    public float sprintMultiplier = 1.6f;  

    [Header("Zıplama ve Yere İniş (Jump & Land)")]
    public float jumpBobAmount = 0.08f;     
    public float landBobMultiplier = 0.015f;
    public float maxLandBob = 0.25f;        
    public float recoverySpeed = 8f;        
    
    [Header("Referanslar")]
    public CharacterController controller; 
    
    private float defaultPosY;
    private float defaultPosX;
    private float timer;
    
    

    [Header("Havadaki Süzülme Ayarları")]
    [Tooltip("Dikey hızın kamerayı ne kadar çekeceği katsayısı")]
    public float verticalLagMultiplier = 0.008f; 
    
    [Tooltip("Kameranın yukarı veya aşağı maksimum ne kadar esneyebileceği")]
    public float maxAirOffset = 0.12f;

    [Tooltip("Süspansiyon yaylanma süresi. Değer büyüdükçe süzülme daha tembel/ağır olur.")]
    public float suspensionSmoothTime = 0.2f;

    private float currentYOffset;
    private float yVelocityRef;
    
    
    
    
    void Start()
    {
        defaultPosY = transform.localPosition.y;
        defaultPosX = transform.localPosition.x;
        
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        if (currentSpeed > 0.1f && controller.isGrounded)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float speedMod = isSprinting ? sprintMultiplier : 1f;

            timer += Time.deltaTime * walkingSpeed * speedMod;

            
            float newY = defaultPosY + Mathf.Sin(timer) * (bobAmountY * speedMod);
            float newX = defaultPosX + Mathf.Cos(timer / 2) * (bobAmountX * speedMod);

            transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
            
            
        }
        

            
        
        
        else
        {
            timer = 0;
             Vector3 targetPosition = new Vector3(defaultPosX, defaultPosY, transform.localPosition.z);
             transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 6f);
        }
        
        
        float targetOffset = 0f;

        if (!controller.isGrounded)
        {
            
            targetOffset = -controller.velocity.y * verticalLagMultiplier;
            targetOffset = Mathf.Clamp(targetOffset, -maxAirOffset, maxAirOffset);
        }
        else
        {
            targetOffset = 0f;
        }

        
        currentYOffset = Mathf.SmoothDamp(
            currentYOffset, 
            targetOffset, 
            ref yVelocityRef, 
            suspensionSmoothTime
        );

        Vector3 currentPos = transform.localPosition;
        currentPos.y = defaultPosY + currentYOffset;
        transform.localPosition = currentPos;
    }
    
    
    
}



