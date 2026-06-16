using UnityEngine;
using Photon.Pun;


public abstract class HealtBase : MonoBehaviourPun,IDamageable
{
    
    public int maxHealt = 100;
    public int healt;
    public bool isDead ;
    

    public void Start()
    {
        healt = maxHealt;
        isDead = false;
        updateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        photonView.RPC("TakeDamageRPC", RpcTarget.AllBuffered, damage);
    }
    
    [PunRPC]
    public void TakeDamageRPC(int damage)
    {
        
        healt -= damage;
        healt = Mathf.Max(0, healt);
        
        updateUI();
        if (healt <= 0) 
        {
            isDead = true; 
            deathLogic();
        }
        
    }
    protected abstract void deathLogic();
    protected virtual void updateUI() { }
    
}
