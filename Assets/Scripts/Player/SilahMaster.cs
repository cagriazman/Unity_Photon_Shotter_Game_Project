using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class SilahMaster : MonoBehaviourPunCallbacks
{
    [Header("FPS Silahları (Sadece Sen Görürsün)")]
    public GameObject[] fpsSilahlar;

    public TPSIKController TpsıkController;
    
    [Header("TPS Silahları (Başkaları Seni Görür)")]
    public GameObject[] tpsSilahlar;

    private int suAnkiSilahIndex = -1; 

    void Start()
    {
        Guncelle();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == photonView.Owner && changedProps.ContainsKey("SecilenSilah"))
        {
            Guncelle();
            
        }
    }

    void Guncelle()
    {
        
        if (photonView.Owner.CustomProperties.TryGetValue("SecilenSilah", out object indexObj))
        {
            int yeniIndex = (int)indexObj;

            
            if (yeniIndex == suAnkiSilahIndex) return;

            
            if (photonView.IsMine)
            {
                
                SetiniKompleKapat(tpsSilahlar);
                
                SilahSetiniAktifEt(fpsSilahlar, yeniIndex);
            }
            else
            {
                SetiniKompleKapat(fpsSilahlar);
                
                SilahSetiniAktifEt(tpsSilahlar, yeniIndex);
            }

            suAnkiSilahIndex = yeniIndex;
            TpsıkController.NewGunSetup(yeniIndex);
        }
    }

    void SilahSetiniAktifEt(GameObject[] liste, int aktifIndex)
    {
        for (int i = 0; i < liste.Length; i++)
        {
            if (liste[i] != null)
                liste[i].SetActive(i == aktifIndex);
        }
    }

    void SetiniKompleKapat(GameObject[] liste)
    {
        foreach (var obj in liste)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}