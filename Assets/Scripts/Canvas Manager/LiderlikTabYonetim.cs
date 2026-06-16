using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
public class LiderlikTabYonetim : MonoBehaviour
{
    public GameObject liderliktabUI;
    [Space]
    public Transform oyuncuItemPrefabParent;
    public GameObject oyuncuItemPrefab;

    private void Start()
    {
        InvokeRepeating(nameof(SkorlariGuncelle), .5f, .5f);
    }
    void Update()
    {
        liderliktabUI.SetActive(Input.GetKey(KeyCode.Tab));
    }
    void SkorlariGuncelle()
    {
        for (int i = oyuncuItemPrefabParent.childCount - 1; i >= 0; i--)
        {
            Destroy(oyuncuItemPrefabParent.GetChild(i).gameObject);
        }


        var SiraliOyuncuListesi =

        (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();


        foreach (var _player in SiraliOyuncuListesi)
        {
            GameObject playerItem = Instantiate(oyuncuItemPrefab, oyuncuItemPrefabParent);
            string _nickname = _player.NickName;
            bool _isMe = _player.UserId == PhotonNetwork.LocalPlayer.UserId;
            if (_isMe) { _nickname = "<color=#FFC200>" + _nickname + "</color>"; }

            playerItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _nickname;
            playerItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _player.GetScore().ToString();
            int _kills = 0;
            int _deaths = 0;
            if (_player.CustomProperties["kills"] != null) _kills = (int)_player.CustomProperties["kills"];
            if (_player.CustomProperties["deaths"] != null) _deaths = (int)_player.CustomProperties["deaths"];

            playerItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _kills + "/" + _deaths;

        }
    }
}
