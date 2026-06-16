
using UnityEngine;
using Photon.Pun;

public class aiWeapon : MonoBehaviourPun
{
    [Header("Sound")]
    public AudioClip[] shootSFX;
    public AudioSource shootSource;
    
    [Header("Namlu Flaş ")]
    public GameObject namluFlasi;
    [Tooltip("FPS Kameranın içindeki silahın ucu (Sadece sen görürsün)")]
    public Transform fpsNamlu;
    
    [Header("Silah özellikleri"),SerializeField]
    protected SilahOzellikleriOS silahOzellikleriOS;
    protected weaponStruct WeaponStruct;

    [Header("Vurulan Nokta Efektleri")]
    public GameObject betonVurusEfekti;
    public GameObject kanSicramaEfekti;
    
    [Header("Ses / UI")]
    [Space] public byte atesSesIndex;

    public TrailRenderer tracerEffect;
    
    [Header("Raycast layer ")]
    public LayerMask vurulabilirLayerlar;
    
    private AiHealt aiHealt;
    public PhotonView pw;

    public Transform aimPos;
    
    
     private void Start()
     {
         WeaponStruct = new weaponStruct(silahOzellikleriOS);
         pw = GetComponentInParent<PhotonView>();
     }

     public void CallFire()
     {
         if (Time.time >= WeaponStruct.siradakiAtisZamani )
         {
             WeaponStruct.siradakiAtisZamani = Time.time + (1f / silahOzellikleriOS.atesHizi);
             AtesEt();
         }
     }

     private void AtesEt()
    {
        
        
        
        WeaponStruct.mevcutMermi--;

        PlayMuzzleEffect(WeaponStruct.silahTuru);
        PlayShootSource(atesSesIndex);
        


        for (int i = 0; i < silahOzellikleriOS.sacmaSayisi; i++)
        {
            var tracer = Instantiate(tracerEffect,aimPos);
            tracer.AddPosition(aimPos.position);
            
            Vector3 yon =  SilahFizigi.RastgeleYonHesapla(aimPos, silahOzellikleriOS.mevcutSacilma);
            Ray isin = new Ray(aimPos.position, yon);
            Vector3 vurusNoktasi;
            
            //carpma yakalarsa
            if (Physics.Raycast(isin, out RaycastHit hit, silahOzellikleriOS.menzil, vurulabilirLayerlar))
            {
                vurusNoktasi = hit.point;
                Quaternion rotasyon = Quaternion.LookRotation(hit.normal);
                Vector3 mermiGidisYonu = isin.direction;
                
                tracer.transform.position = vurusNoktasi;

                if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Ai") )
                {
                    //PhotonView hitHasPw = hit.transform.GetComponentInParent<PhotonView>();
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

                        //bool buVurusOldururMu = (hitHealt.healt - silahOzellikleriOS.vurusHasari) <= 0;
                        
                        // int benimTakim = 0;
                        // if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Takim"))
                        //     benimTakim = (int)PhotonNetwork.LocalPlayer.CustomProperties["Takim"];
                        //
                        //
                        // int hedefTakim = 0;
                        // if (hitHasPw.Owner.CustomProperties.ContainsKey("Takim"))
                        //     hedefTakim = (int)hitHasPw.Owner.CustomProperties["Takim"];
                        //
                        // if (benimTakim == hedefTakim )
                        // {
                        //     Debug.Log("<color=blue>Aga dur! Kendi adamını vuruyorsun!</color>");
                        //     return; 
                        // }
                        //

                        if (hitObject!=null)
                        {
                            hitObject.TakeDamage(silahOzellikleriOS.vurusHasari);
                        }


                        // if (buVurusOldururMu)
                        // {
                        //     VurulduScripti.animOldu(hitHasPw.Owner.NickName);
                        // }
                        // else
                        // {
                        //     VurulduScripti.Vurduk(silahOzellikleriOS.vurusHasari);
                        // }
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
                //vurusNoktasi = aimPos.position + (aimPos.forward * 100f);
            } 
            
            
            //photonView.RPC("RPC_MermiIzi", RpcTarget.All, vurusNoktasi,WeaponStruct.silahTuru);
            
            
          
        }
    }
    public void PlayShootSource(byte _index)
    {
        shootSource.clip = shootSFX[_index];
        shootSource.Stop();
        shootSource.Play();
    }
    public void PlayMuzzleEffect(int namluYeri)
    {
        Transform kullanilacakNokta = fpsNamlu;

        if (namluFlasi && kullanilacakNokta)
        {
            GameObject flash = Instantiate(namluFlasi, kullanilacakNokta.position, kullanilacakNokta.rotation);
            flash.transform.SetParent(kullanilacakNokta);
            Destroy(flash, 0.1f);
        }
    }

    
}


