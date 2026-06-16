using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Photon.Pun;



public class ChatYonetim : MonoBehaviourPun
{
    [Header("Chat UI Elemanları")]
    public TextMeshProUGUI chatText;
    public TMP_InputField yazmaAlani;
    
    public GameObject chatPanel;

    public static bool chatAcikMi;

    private bool inputFieldAktifMi;

    void Start()
    {
        if (chatPanel != null) chatPanel.SetActive(false);
        inputFieldAktifMi = false;
        chatAcikMi = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && !inputFieldAktifMi)
        {
            ChatiAc();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && inputFieldAktifMi)
        {
            yazmaAlani.text = ""; 
            ChatiKapat();
        }
        bool enterBasildi = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

        if (enterBasildi && inputFieldAktifMi)
        {
            if (!string.IsNullOrEmpty(yazmaAlani.text))
            {
                photonView.RPC("PRC_LokalOyuncuMesajGonder", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, yazmaAlani.text);
            }
            yazmaAlani.text = ""; 
            ChatiKapat(); 
        }
    }
    private void ChatiAc()
    {
        inputFieldAktifMi = true;
        chatAcikMi = true; 
        
        if (chatPanel != null) chatPanel.SetActive(true);
        yazmaAlani.Select();
        yazmaAlani.ActivateInputField();
    }

    private void ChatiKapat()
    {
        inputFieldAktifMi = false;
        chatAcikMi = false; 
        
        if (chatPanel != null) chatPanel.SetActive(false);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
    [PunRPC]
    public void PRC_LokalOyuncuMesajGonder(string _OyuncuIsim, string _chatMesaj)
    {
        string _mesajEkle = "<b>" + _OyuncuIsim + ":</b>" + _chatMesaj;

        chatText.text = _mesajEkle + "\n" + chatText.text;
    }

}
