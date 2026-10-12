using System;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Rendering;
using StarterAssets;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

using UnityEngine.UI;

public class OyuncuIcın : MonoBehaviourPunCallbacks
{
    [Header("Kafa İkonu Ayarı")]
    public Image kafaIkonuResmi;
    
    [Header("Kamera Ayarları")]
    public GameObject sanalKamera; 
    public GameObject tpCamera;

    [Header("Kontrolcü")]
    public FirstPersonController fpc;
    public GameObject oyuncuUI;

    [Header("İsim ve Görünüm (TP - Dışarıdan Görünen)")]
    public TextMeshProUGUI nameText;
    public GameObject tpOyuncuGorunum; 
    public int teamNumber;
    
    
    [Header("Takım Materyal Paketleri")]
    public Material[] saldiriPaketi;  
    public Material[] savunmaPaketi;
    
    private StarterAssetsInputs _input;
    public SkinnedMeshRenderer meshRenderer;
    [Header("BOMB")]
    public GameObject bomb;
    public bool canPlant=false;
    private float bombTimer;
    private bool oneTimePlant = true;
    public Image bombaBarImage;
    [Header("DEFUSE")]
    public bool canDefuse = false;
    private float defuseTimer;
    private bool oneTimeDefuse = true;
    public float defuseSuresi = 5.0f;

    
    private void Start()
    {
            _input = GetComponent<StarterAssetsInputs>();
            if (sanalKamera != null) sanalKamera.SetActive(photonView.IsMine);
            if (fpc != null) fpc.enabled = photonView.IsMine;
            if (oyuncuUI != null) oyuncuUI.SetActive(photonView.IsMine);
            if (tpCamera != null) tpCamera.SetActive(photonView.IsMine);
            if(kafaIkonuResmi!=null&&photonView.IsMine) kafaIkonuResmi.color=Color.gold;
        
            if (nameText != null)
            {
                nameText.text = photonView.Owner.NickName;
                nameText.gameObject.SetActive(!photonView.IsMine);
            }

        
            
            if (photonView.Owner.CustomProperties.TryGetValue("Takim", out object takimNo))
            {
                GorseliUygula((int)takimNo);
                teamNumber = (int)takimNo;
            }
            else
            {
                GorseliUygula(1); 
            }

        
            if (tpOyuncuGorunum != null)
            {
            
                tpOyuncuGorunum.SetActive(true);
            
                if (photonView.IsMine)
                {
                
                    Renderer[] parcalar = tpOyuncuGorunum.GetComponentsInChildren<Renderer>();
                    foreach (Renderer r in parcalar)
                    {
                        r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    }
            
                
                    SetLayerRecursively(tpOyuncuGorunum, 2); 
                }
            
            }
            bombaBarImage.gameObject.SetActive(false);
            bombaBarImage.fillAmount = 0f;
            
    }

    private void Update()
    {
        actionBind();
    }

    private void actionBind()
    {
        if (_input == null) return;
        
        
        if (_input.plant && canPlant && bomb.activeSelf&&oneTimePlant&&teamNumber == 0)
        { 
            bombaBarImage.gameObject.SetActive(true);
            bombaBarImage.color = Color.red; 
            bombTimer += Time.deltaTime;
            bombaBarImage.fillAmount = bombTimer / 4.0f;
            
            if (bombTimer > 4.0f)
            {
                photonView.RPC("PlantBomb", RpcTarget.AllBuffered);

                bombTimer = 0;
                oneTimePlant = false;
                if (RoundYonetimi.Instance != null)
                {
                    RoundYonetimi.Instance.BombaKurulduTetikle();
                }

            }
        }
        else if (_input.plant &&  teamNumber == 1 && oneTimeDefuse)
        {
            bombaBarImage.gameObject.SetActive(true);
            bombaBarImage.color = Color.blue; 
            
            defuseTimer += Time.deltaTime;
            bombaBarImage.fillAmount = defuseTimer / defuseSuresi;
            
            if (defuseTimer > defuseSuresi)
            {
                defuseTimer = 0;
                oneTimeDefuse = false;
                
                
                if (RoundYonetimi.Instance != null)
                {
                    RoundYonetimi.Instance.BombaImhaEdildiTetikle(); 
                }
            }
        }
        else
        {
            bombTimer = 0;
            bombaBarImage.gameObject.SetActive(false);

        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer != null && targetPlayer == photonView.Owner)
        {
            if (changedProps.ContainsKey("Takim"))
            {
                int yeniTakim = (int)changedProps["Takim"];
                //Debug.Log($"{targetPlayer.NickName} takımı güncellendi: {yeniTakim}");
                GorseliUygula(yeniTakim);
                teamNumber = yeniTakim;
                

            }
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    void GorseliUygula(int takimNo)
    {
        if (meshRenderer == null) return;
        if (takimNo == 0)
        {
            meshRenderer.sharedMaterials = saldiriPaketi; 
            if(!photonView.IsMine)kafaIkonuResmi.color=Color.black;
        }
        else if (takimNo == 1)
        {
            meshRenderer.sharedMaterials = savunmaPaketi;
            if(!photonView.IsMine)kafaIkonuResmi.color=Color.white;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomb"))
        {
           photonView.RPC("TakeBombRPC", RpcTarget.AllBuffered);
           other.gameObject.SetActive(false);
        }
    }

    [PunRPC] public void TakeBombRPC() { bomb.SetActive(true); }
    [PunRPC] public void PlantBomb()
    { 
        bomb.transform.SetParent(null);
        bomb.transform.position = transform.position; 
        bomb.transform.rotation = Quaternion.identity; 
    }
   
}