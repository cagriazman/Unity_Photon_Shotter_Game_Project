using System.Collections;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class YonetimScript : MonoBehaviourPunCallbacks
{

    public string odaKod = "harita1";
    [Header("Arayüz Elemanları")]
    public GameObject Oyuncu;
    public GameObject OdaCam;
    public static YonetimScript Instance;
    private string currentName;
    public GameObject baglanCanvasi;
    public GameObject takimSecCanvasi;

    public Transform[] spawnNoktalari;
    private int spawnIndex;
    private GameObject benimKarakterim;
    private Transform sp;

    public void HaritaSec(int haritaIndikatoru)
    {
        spawnIndex = haritaIndikatoru;
    }

    Transform SiradakiSpawn(int takimSpawnNok) 
    {
        int finalIndex = (spawnIndex * 2) + takimSpawnNok;

        if (finalIndex < spawnNoktalari.Length) return spawnNoktalari[finalIndex];
        
        return spawnNoktalari[0];
    }

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    public void ChangeName(string _name)
    {
        currentName = _name;
    }
     public void ConnectToServer()
    {
        Debug.Log("Bağlanıyor..");
        //baglanCanvasi.SetActive(true);

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Lobiye Bağlanıyor..");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Oda oluşturuluyor");
        PhotonNetwork.JoinOrCreateRoom(odaKod, null, null);
    }

    public override void OnJoinedRoom()
    {
        
        Debug.Log("Bağlandı..");
        PhotonNetwork.LocalPlayer.NickName = currentName;
        SpawnChar();
        baglanCanvasi.SetActive(false);
        

    }
    public void YenidenDog()
    {
        StartCoroutine(RespawnGecikmesi());
    }
    IEnumerator RespawnGecikmesi()
    {
        if (benimKarakterim != null) PhotonNetwork.Destroy(benimKarakterim);
        yield return new WaitForSeconds(0.5f);
        SpawnChar();
    }
    
    public void SpawnChar()
    {
        benimKarakterim = PhotonNetwork.Instantiate(Oyuncu.name, Vector3.up * 100, Quaternion.identity);
        CursorKontrol(false);
        OdaCam.SetActive(true);
        takimSecCanvasi.SetActive(true);
    }
    public void TakimSecVeOyunaGir(int secilenTakim)
    {
        Hashtable oyuncuKimligi = new Hashtable { ["Takim"] = secilenTakim };
        PhotonNetwork.LocalPlayer.SetCustomProperties(oyuncuKimligi);
        sp= SiradakiSpawn(secilenTakim);
    }

    public void SilahSec(int secilenSilah)
    {
        Hashtable oyuncuKimligi = new Hashtable {{ "SecilenSilah", secilenSilah }};
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(oyuncuKimligi);
        
        OdaCam.SetActive(false);
        if (benimKarakterim != null)
        {
            CharacterController cc = benimKarakterim.GetComponent<CharacterController>();
            if(cc != null) cc.enabled = false;

            benimKarakterim.transform.position = sp.position;
            benimKarakterim.transform.rotation = sp.rotation;

            if(cc != null) cc.enabled = true;
            CursorKontrol(true);
            
        }
    }
    
    public void CursorKontrol(bool kilitliMi)
    {
        if (kilitliMi)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


}



