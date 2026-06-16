using System;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] rigidbodies;
    [SerializeField]
    Animator animator;

    public Transform meshSocket;
    
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
    }

    public void getRagdolls(bool key)
    {
        realeseWeapon();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = key;
        }
        animator.enabled = key;
    }

    public void realeseWeapon()
    {
        meshSocket.SetParent(null);
        Rigidbody rb = meshSocket.transform.GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
    
}
