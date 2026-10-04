using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using StarterAssets;
using UnityEngine.InputSystem;

public abstract class SilahTaban : MonoBehaviourPun
{
    [Header("Silah özellikleri OSsi"),SerializeField]
    protected SilahOzellikleriOS silahOzellikleriOS;
    protected weaponStruct WeaponStruct;
    
    [Header("Raycast layer ")]
    public LayerMask vurulabilirLayerlar;

    [Header("Animator")]
    public Animator anim;
    

    [Header("Scriptler")]
    public VurulduVeOldu VurulduScripti;
    public AnimasyonSenkronizasyon _animasyonSenkronizasyon;
    
    [Space]
    public TextMeshProUGUI mermiYazi;
    public Image mermiGostergesi;

    [Header("Vurulan Nokta Efektleri")]
    public GameObject betonVurusEfekti;
    public GameObject kanSicramaEfekti;

    [Header("Ses / UI")]
    [Space] public byte atesSesIndex;
    public PhotonOyucuSesYonetim photonOyucuSesYonetim;
    
    [Header("GameObjectler")]
    public GameObject crosshair;
    [SerializeField]
    private Camera kamera;
    public PhotonView pw;
    public Sway swayScript;

    protected bool isReloading;
    protected bool ads;

    private AiHealt aiHealt;
    [SerializeField]
    private StarterAssetsInputs _input;
    
    protected camRecoil  camRecoil;
    
    private CharacterController controller; 
    [SerializeField] private float recoilTimer=0;
    private float sensitivityForRecoil;

    protected virtual void Start()
    {
        camRecoil=GetComponentInParent<camRecoil>();
        _input = GetComponentInParent<StarterAssetsInputs>();
        pw = GetComponentInParent<PhotonView>();
        kamera = Camera.main;
        controller= GetComponentInParent<CharacterController>();
        if (silahOzellikleriOS != null)
        {
            WeaponStruct = new weaponStruct(silahOzellikleriOS);
        }
        MermiUIGuncelle();
    }

    protected virtual void Update()
    {
        if (!pw.IsMine) return;
        HandleInput();
    }

    protected virtual void HandleInput()
    {
         ads = _input.getAim;
        anim.SetBool("ADS", ads);

        bool shift_aktif = _input.sprint;
        anim.SetBool("isRunHeld", shift_aktif);

        if (isReloading) return;
        
        //sacilma ayari
        if (myPyhsic.hasSpeed(&controller))
        {
            recoilTimer += Time.deltaTime*sensitivityForRecoil;
            silahOzellikleriOS.mevcutSacilma += silahOzellikleriOS.sacilma * recoilTimer;
        }
        else
        {
            
            recoilTimer = 0f;
            silahOzellikleriOS.mevcutSacilma = 0f;
        }
        
        
        
        
        if (ads)
        {
            crosshair.SetActive(false);
            if (swayScript) swayScript.isAiming = true;
            //silahOzellikleriOS.mevcutSacilma=silahOzellikleriOS.sacilma/2;
        }
        if (!ads)
        {   
            crosshair.SetActive(true);
            swayScript.isAiming = false;
            //silahOzellikleriOS.mevcutSacilma = silahOzellikleriOS.sacilma;
        }

        // Ateş cagir
        if (Mouse.current.leftButton.isPressed && Time.time >= WeaponStruct.siradakiAtisZamani && WeaponStruct.mevcutMermi > 0 && !shift_aktif)
        {
            WeaponStruct.siradakiAtisZamani = Time.time + (1f / silahOzellikleriOS.atesHizi);
            AtesEt();
        }

        // Reload cagir
        if ((Input.GetKeyDown(KeyCode.R) || WeaponStruct.mevcutMermi <= 0) && WeaponStruct.yedekMermi > 0 && WeaponStruct.mevcutMermi < WeaponStruct.sarjorKapasitesi)
        {
            Reload();
        }
    }

    protected virtual void Reload()
    {
        StartCoroutine(ReloadCoroutine());
        photonView.RPC("RPC_SarjorDegis", RpcTarget.Others);
    }

    protected void MermiMatematiginiYap(int miktar)
    {
        int eklenecek = Mathf.Min(miktar, WeaponStruct.yedekMermi);
        WeaponStruct.mevcutMermi += eklenecek;
        WeaponStruct.yedekMermi -= eklenecek;
        MermiUIGuncelle();
    }

    #region  Mermi Degisimi Fonk

    IEnumerator ReloadCoroutine()
    {
        anim.SetBool("IsReloading", true);
        _animasyonSenkronizasyon.IKvalue = 0f;
        isReloading = true;
        anim.SetTrigger("Reload");


        yield return new WaitForSeconds(silahOzellikleriOS.reloadSure);


        int ihtiyac = WeaponStruct.sarjorKapasitesi - WeaponStruct.mevcutMermi;
        MermiMatematiginiYap(ihtiyac);

        MermiUIGuncelle();

        anim.SetBool("IsReloading", false);
        isReloading = false;
        _animasyonSenkronizasyon.IKvalue = 1f;

    }
    #endregion

    protected abstract void AtesAnimasyonuOynat();

    protected virtual void callRecoil()
    {
        camRecoil.Recoil(silahOzellikleriOS.recoilTime, silahOzellikleriOS.recoilPower, silahOzellikleriOS.downcharRotationSpeed,silahOzellikleriOS.upcharRotationSpeed);
        
    }
    
    #region Ateş etme fonk
    
    protected virtual void AtesEt()
    {
        
        callRecoil();
        
        WeaponStruct.mevcutMermi--;
        MermiUIGuncelle();

        photonOyucuSesYonetim.photonView.RPC("RPC_NamluFlasi", RpcTarget.All,WeaponStruct.silahTuru);
        photonOyucuSesYonetim.photonView.RPC("RPC_PlayShootSource", RpcTarget.All, atesSesIndex);

        AtesAnimasyonuOynat();


        for (int i = 0; i < silahOzellikleriOS.sacmaSayisi; i++)
        { 
            
            Vector3 yon =  myPyhsic.calculateDir(kamera.transform, silahOzellikleriOS.mevcutSacilma);
            Ray isin = new Ray(kamera.transform.position, yon);
            Vector3 vurusNoktasi;

            //carpma yakalarsa
            if (Physics.Raycast(isin, out RaycastHit hit, silahOzellikleriOS.menzil, vurulabilirLayerlar))
            {
                vurusNoktasi = hit.point;
                Quaternion rotasyon = Quaternion.LookRotation(hit.normal);
                Vector3 mermiGidisYonu = isin.direction;

                if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Ai") )
                {
                    PhotonView hitHasPw = hit.transform.GetComponentInParent<PhotonView>();
                    PlayerHealt hitHealt = hit.transform.GetComponentInParent<PlayerHealt>();
                    aiHealt= hit.transform.GetComponentInParent<AiHealt>();
                    Rigidbody rb =  hit.transform.GetComponent<Rigidbody>();
                    IDamageable hitObject = hit.transform.root.GetComponentInChildren<IDamageable>();

                    //ai alanı baslayis
                        if (rb!=null)
                    {
                        float silahFirlamaGucu = 12f;
                        Vector3 firlatmaVektoru = mermiGidisYonu.normalized * 0.3f * silahFirlamaGucu;
                        rb.AddForce(firlatmaVektoru, ForceMode.Impulse);
                    }
                        
                    if (aiHealt != null )
                    {
                        if (kanSicramaEfekti) PhotonNetwork.Instantiate(kanSicramaEfekti.name, hit.point, rotasyon);
                        hitObject.TakeDamage(silahOzellikleriOS.vurusHasari);

                    }
                    //ai alanı bitisi
                    
                    
                    
                    if (hitHealt != null && hitHealt.healt>0)
                    {

                        bool buVurusOldururMu = (hitHealt.healt - silahOzellikleriOS.vurusHasari) <= 0;
                        
                        int benimTakim = 0;
                        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Takim"))
                            benimTakim = (int)PhotonNetwork.LocalPlayer.CustomProperties["Takim"];
                        
                        
                        int hedefTakim = 0;
                        if (hitHasPw.Owner.CustomProperties.ContainsKey("Takim"))
                            hedefTakim = (int)hitHasPw.Owner.CustomProperties["Takim"];

                        if (benimTakim == hedefTakim )
                        {
                            Debug.Log("<color=blue>Aga dur! Kendi adamını vuruyorsun!</color>");
                            return; 
                        }
                        

                        if (hitHasPw!=null)
                        {
                            hitObject.TakeDamage(silahOzellikleriOS.vurusHasari);
                        }


                        if (buVurusOldururMu)
                        {
                            VurulduScripti.animOldu(hitHasPw.Owner.NickName);
                        }
                        else
                        {
                            VurulduScripti.Vurduk(silahOzellikleriOS.vurusHasari);
                        }
                        if (kanSicramaEfekti) PhotonNetwork.Instantiate(kanSicramaEfekti.name, hit.point, rotasyon);
                    }
                }
                else
                {
                    PhotonNetwork.Instantiate(betonVurusEfekti.name, hit.point, rotasyon);
                }
            }
            //carpma yakalayamassa
            else
            {
                vurusNoktasi = kamera.transform.position + (kamera.transform.forward * 100f);
            } 
            
            
            if (photonOyucuSesYonetim != null) photonOyucuSesYonetim.photonView.
                RPC("RPC_MermiIzi", RpcTarget.All, vurusNoktasi,WeaponStruct.silahTuru);
            
            
          
        }
    }


    #endregion

    protected virtual void MermiUIGuncelle()
    {
        if (mermiYazi) mermiYazi.text = $"<b>{WeaponStruct.mevcutMermi}/</b>{WeaponStruct.yedekMermi}";
        if (mermiGostergesi) mermiGostergesi.fillAmount = (float)WeaponStruct.mevcutMermi / WeaponStruct.sarjorKapasitesi;
    }
    
    

}
