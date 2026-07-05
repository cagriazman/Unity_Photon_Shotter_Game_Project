using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class WeaponIK : MonoBehaviour
{
    [SerializeField]
    Transform target;
    public Transform aim;
    public Transform bone;
    public float maxAngle=30.0f;
    public float minDistance=1.0f;
    public float charRotationSpeed = 2.5f;
    [Header("Dönüş Hesaplama Elemanları")]
    [SerializeField]private float _zamanlayici = 0f;
    [SerializeField]private float _referansOrigin=0f;
    public float HorizontalForMouse;
    [Space]
    private Quaternion lastRotation;

    [SerializeField] private float angleCounter;
    
    [SerializeField]
    AiAgent agent;
    public Vector3 additionalOffset;
    void Start()
    {
        agent = GetComponent<AiAgent>();
    }
    
    void RotateCharacterIfNeeded()
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0; 
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, charRotationSpeed * Time.deltaTime);
        
    }

    float RotationForAnim()
    {
        _zamanlayici += Time.deltaTime;

        float anlikAciFarki = Mathf.DeltaAngle(_referansOrigin, transform.eulerAngles.y);
            
        if (_zamanlayici >= 1f)
        {
            _referansOrigin = transform.eulerAngles.y;
            _zamanlayici = 0f;
        }
        HorizontalForMouse = Mathf.Lerp(HorizontalForMouse, 2 * anlikAciFarki, Time.deltaTime * 3f);
        return HorizontalForMouse;

    }
    
    Vector3 GetAimPosition()
    {
        Vector3 targetDirection = (target.position+additionalOffset) - aim.position;
        Vector3 aimDirection = aim.forward;

        float targetBlendOut = 0.0f;
        float targetDistance = targetDirection.magnitude;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > maxAngle )
        {
            targetBlendOut += (targetAngle-maxAngle)/50.0f;
            RotateCharacterIfNeeded();
            agent.animator.SetFloat("Ai_fareIcinYtyYn", RotationForAnim());
        }
        else
        {
            float value = Mathf.Lerp(RotationForAnim(), 0.0f,Time.deltaTime * 1f);
            agent.animator.SetFloat("Ai_fareIcinYtyYn", value);
        }

        if (targetDistance < minDistance)
        {
            targetBlendOut += minDistance-targetDistance;
            
        }
        
        targetBlendOut = Mathf.Clamp01(targetBlendOut);
        
        
        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, targetBlendOut);
        return direction;
    }

    void LateUpdate()
    {           
        target = agent.playerTransform;
        //GameObject go = agent.playerTransform.gameObject;
        if (target != null)        AimToTarget(bone,GetAimPosition());
        else AimToTarget(bone,transform.forward);
    }

    private void AimToTarget(Transform bone1, Vector3 targetDir)
    {
        Vector3 aimDirection = aim.forward;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection,targetDir);
        bone1.rotation= aimTowards *  bone1.rotation;
    }
    
}
