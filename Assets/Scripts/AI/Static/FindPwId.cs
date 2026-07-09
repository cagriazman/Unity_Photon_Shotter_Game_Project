using Photon.Pun;
using UnityEngine;

public static class FindPwId 
{
    public static int GetViewId(GameObject obj)
    {
        if (obj == null) return -1; 

        PhotonView pwNewObject = obj.GetComponentInParent<PhotonView>();
        
        if (pwNewObject != null)
        {
            return pwNewObject.ViewID;
        }
        
        return -1; 
    }
}