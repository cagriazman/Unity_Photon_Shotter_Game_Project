using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TPSIKController : MonoBehaviour
{
    [Header("IK Bileşenleri")]
    [SerializeField] private Transform[] targetWeapon;
    [SerializeField] private Transform targetLeftArm;
    [SerializeField] private Transform targetRightArm;

    public TwoBoneIKConstraint leftElIKConstraint;
    public TwoBoneIKConstraint rightElIKConstraint;
    public MultiAimConstraint silahMultiAimConstraint;
    

    public void NewGunSetup(int index)
    {
        
        
         targetRightArm =targetWeapon[index].SearchAndFindInChildren("IK_rightArm");
         targetLeftArm = targetWeapon[index].SearchAndFindInChildren("IK_leftArm");
         
         if (targetLeftArm != null && leftElIKConstraint != null)
         {
             var data = leftElIKConstraint.data;
             data.target = targetLeftArm; 
             leftElIKConstraint.data = data;
         }

         if (targetRightArm != null && rightElIKConstraint != null)
         {
             var data = rightElIKConstraint.data;
             data.target = targetRightArm;
             rightElIKConstraint.data = data;
         }

         if (targetWeapon != null && silahMultiAimConstraint != null)
         {
             var aimData = silahMultiAimConstraint.data;
            
             aimData.constrainedObject = targetWeapon[index];

             silahMultiAimConstraint.data = aimData;
         }

         RigBuilder rigBuilder = GetComponentInParent<RigBuilder>();
         if (rigBuilder != null && (targetLeftArm != null || targetRightArm != null || targetWeapon != null))
         {
             rigBuilder.Build(); 
         }
         
        
        
        
    }

    
}