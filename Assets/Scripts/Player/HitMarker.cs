using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class HitMarker : MonoBehaviour
{
    [Header("UI")]
    public Animation hitmarkeranim;
    public AudioSource hitMarkerSes;
    [Space]
    public Animation killmarkeranim;
    public AudioSource killmarkerSes;

    public void playWasShot(int _vurusHasari)
    {
        hitmarkeranim.Stop();
        hitmarkeranim.Play();


        hitMarkerSes.Stop();
        hitMarkerSes.Play();
        PhotonNetwork.LocalPlayer.AddScore(_vurusHasari);
    }
    public void playDeath(string _oldurulen)
    {
        killmarkeranim.Stop();
        killmarkeranim.Play();


        killmarkerSes.Stop();
        killmarkerSes.Play();
        LokalKDYonetim.Instance.GetKill();

        BilidirimYonetim.Instance.photonView.RPC("RPC_bildirimVer", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, _oldurulen);
    }
}
