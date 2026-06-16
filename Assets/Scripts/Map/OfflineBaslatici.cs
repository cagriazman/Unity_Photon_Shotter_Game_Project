using UnityEngine;
using Photon.Pun;

public class OfflineBaslatici : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("LocalTest");
        }
    }

    
}