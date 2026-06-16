using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TimeObjectDestructor : MonoBehaviourPun
{
    public float lifetime;
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DelayedDestroy());
        }
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        PhotonNetwork.Destroy(gameObject);
    }
    
}
