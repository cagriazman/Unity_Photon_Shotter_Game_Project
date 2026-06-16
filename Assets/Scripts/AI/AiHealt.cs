using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class AiHealt : HealtBase
{
    
     
    RagdollController ragdollController;
    AiAgent aiAgent;
    private WeaponIK weaponIK;
    

    public void Start()
    {
        base.Start();
        weaponIK = GetComponent<WeaponIK>();
        aiAgent = GetComponent<AiAgent>();
        ragdollController = GetComponent<RagdollController>();
       
    }


    protected override void deathLogic()
    {
        if (isDead)
        {
            if (aiAgent)
            {
                ragdollController.getRagdolls(false);
                weaponIK.enabled = false;
                aiAgent.enabled = false;
                StartCoroutine(DestroyEverything());
            }
        }
    }
    IEnumerator DestroyEverything()
    {
        yield return new WaitForSeconds(5f);
        
        Destroy(gameObject);
    } 

    

}
