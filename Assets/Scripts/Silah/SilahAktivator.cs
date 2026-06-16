using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon; 

public class SilahAktivator : MonoBehaviourPunCallbacks
{
    [Header("Silah Objeleri (Sırasıyla At: 0:AK, 1:Shotgun...)")]
    public GameObject[] tumSilahlar;

    void Start()
    {
        SilahiGuncelle();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == photonView.Owner && changedProps.ContainsKey("SecilenSilah"))
        {
            SilahiGuncelle();
        }
    }

    void SilahiGuncelle()
    {
        if (photonView.Owner.CustomProperties.TryGetValue("SecilenSilah", out object indexObj))
        {
            int secilenIndex = (int)indexObj;

            for (int i = 0; i < tumSilahlar.Length; i++)
            {
                
                tumSilahlar[i].SetActive(i == secilenIndex);
            }
        }
    }
}