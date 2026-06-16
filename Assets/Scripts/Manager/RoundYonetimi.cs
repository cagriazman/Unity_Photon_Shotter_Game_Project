
using System.Collections;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoundYonetimi : MonoBehaviourPunCallbacks
{
    public static RoundYonetimi Instance;
    
    [Header("Süre Ayarları")]
    public float roundSuresi = 120f;
    public float bombaSuresi = 40f; 
    public TextMeshProUGUI timerYazi;
    
    [Header("Skor Arayüz Ayarları")]
    public TextMeshProUGUI tSkorYazi;  
    public TextMeshProUGUI ctSkorYazi;
    private int tSkor = 0;
    private int ctSkor = 0;
    
    [Header("Sahne Bombası Ayarları")]
    public GameObject bombInMap; 
    
    private double startTime = 0; 
    private double bombStartTime = 0; 
    
    private bool roundBitti = false;
    public bool bombaKurulduMu = false; 

    void Awake() => Instance = this;

    public override void OnJoinedRoom() => CheckStartTimeFromRoom();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && startTime == 0 && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            StartRound();
        }
    }

    void Start()
    {
        if (PhotonNetwork.InRoom) CheckStartTimeFromRoom();
        SkorUIYazdir();
    }

    private void StartRound()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        startTime = PhotonNetwork.Time; 
        Hashtable props = new Hashtable { { "StartTime", startTime } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void BombaKurulduTetikle()
    {
        double guncelZaman = PhotonNetwork.Time;
        Hashtable props = new Hashtable { { "BombStartTime", guncelZaman } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void CheckStartTimeFromRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("StartTime", out object st))
            {
                startTime = (double)st;
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("BombStartTime", out object bst))
            {
                double bstVal = (double)bst;
                if (bstVal > 0)
                {
                    bombStartTime = bstVal;
                    bombaKurulduMu = true;
                    if (timerYazi != null) timerYazi.color = Color.red; 
                }
                else
                {
                    bombStartTime = 0;
                    bombaKurulduMu = false;
                }
            }
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("StartTime"))
        {
            startTime = (double)propertiesThatChanged["StartTime"];
        }
        
        if (propertiesThatChanged.ContainsKey("BombStartTime"))
        {
            double bstVal = (double)propertiesThatChanged["BombStartTime"];
            if (bstVal > 0)
            {
                bombStartTime = bstVal;
                bombaKurulduMu = true;
                if (timerYazi != null) timerYazi.color = Color.red; 
            }
            else
            {
                bombStartTime = 0;
                bombaKurulduMu = false;
                if (timerYazi != null) timerYazi.color = Color.white;
            }
        }
    }

    void Update()
    {
        if (startTime <= 0) 
        {
            if(timerYazi != null) timerYazi.text = "Waiting for other players...";
            return; 
        }

        
        if (roundBitti) return;

        double kalan;
        if (bombaKurulduMu)
        {
            kalan = bombaSuresi - (PhotonNetwork.Time - bombStartTime);
            if (kalan <= 0) { kalan = 0; EndRoundLocal(); } 
        }
        else
        {
            kalan = roundSuresi - (PhotonNetwork.Time - startTime);
            if (kalan <= 0) { kalan = 0; EndRoundLocal(); } 
        }

        TimerUIGuncelle(kalan);
    }

    void TimerUIGuncelle(double sure)
    {
        int dakika = Mathf.FloorToInt((float)sure / 60);
        int saniye = Mathf.FloorToInt((float)sure % 60);
        
        if (bombaKurulduMu)
        {
            int milisaniye = Mathf.FloorToInt(((float)sure * 100) % 100);
            timerYazi.text = string.Format("{0:00}:{1:00}", saniye, milisaniye);
        }
        else
        {
            timerYazi.text = string.Format("{0:00}:{1:00}", dakika, saniye);
        }
    }

    private void EndRoundLocal()
    {
        if (roundBitti) return;
        roundBitti = true;

        if (bombaKurulduMu)
        {
            tSkor++;
        }
        else
        { 
            ctSkor++;
        }
        SkorUIYazdir();

        KurbanlariTemizleVeGecikmeBaslat();
    }

    public void BombaImhaEdildiTetikle()
    {
        photonView.RPC("RPC_BombaImhaEdildi", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_BombaImhaEdildi()
    {
        if (roundBitti) return;
        roundBitti = true;
        
        ctSkor++;
        SkorUIYazdir();
        
        if (timerYazi != null) 
        {
            timerYazi.color = Color.green; 
            timerYazi.text = "BOMB DEFUSED!";
        }

        KurbanlariTemizleVeGecikmeBaslat();
    }

    private void KurbanlariTemizleVeGecikmeBaslat()
    {
        GameObject kurulanBomba = GameObject.FindWithTag("PlantedBomb");
        if (kurulanBomba != null) Destroy(kurulanBomba); 

        if (bombInMap != null) bombInMap.SetActive(true);

        var haritadakiKurbanlar = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>();
        foreach (IDamageable kurban in haritadakiKurbanlar)
        {
            kurban.TakeDamage(1000); 
        }
        
        StartCoroutine(YeniRounduGecikmeliBaslat(3f));
    }
    
    private IEnumerator YeniRounduGecikmeliBaslat(float gecikmeSuresi)
    {
        yield return new WaitForSeconds(gecikmeSuresi);

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            Hashtable temizlikProps = new Hashtable 
            { 
                { "StartTime", 0.0 }, 
                { "BombStartTime", 0.0 } 
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(temizlikProps);
            
            photonView.RPC("RPC_RounduSifirla", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_RounduSifirla()
    {
        roundBitti = false;
        bombaKurulduMu = false;
        
        if (timerYazi != null) 
        {
            timerYazi.color = Color.white; 
            timerYazi.text = "NEW ROUND STARTING...";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartRound();
        }
    }

    private void SkorUIYazdir()
    {
        if (tSkorYazi != null) tSkorYazi.text = tSkor.ToString();
        if (ctSkorYazi != null) ctSkorYazi.text = ctSkor.ToString();
    }
}