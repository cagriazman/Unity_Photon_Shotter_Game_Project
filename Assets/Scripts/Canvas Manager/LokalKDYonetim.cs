using System;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class LokalKDYonetim : MonoBehaviour
{
    public static LokalKDYonetim Instance;
    
    private void Awake()
    {
        Instance = this;
    }
    public int localplayer_kills;
    public int localplayer_deaths;

    public void GetKill()
    {
        localplayer_kills++;

        SetHashes();

    }
    public void OnDied()
    {
        localplayer_deaths++;

        SetHashes();
    }
    private void SetHashes()
    {
        try
        {
            Hashtable hash = new Hashtable
            {

                ["kills"] = localplayer_kills,
                ["deaths"] = localplayer_deaths
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        finally
        {

        }

    }
}
