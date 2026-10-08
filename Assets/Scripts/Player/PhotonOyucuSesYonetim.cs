using System;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;


public class PhotonOyucuSesYonetim : MonoBehaviourPun
{
    public AudioClip[] shootSFX;
    public AudioSource shootSource;


    [Header("Ayak Sesi ")]
    public AudioSource adimSource;
    public AudioClip[] adimSFX;
    public AudioClip[] ziplaSFX;
    public AudioClip[] dusmeSFX;



    [Header("Reload Sesi ")]
    public AudioSource reloadSource;
    public AudioClip[] reloadSFX;

    [Header("Namlu Flaş ")]
    public GameObject namluFlasi;
    [Tooltip("FPS Kameranın içindeki silahın ucu (Sadece sen görürsün)")]
    public Transform[] fpsNamlu;

    [Tooltip("Karakter modelinin elindeki silahın ucu (Diğerleri bunu görür)")]
    public Transform[] tpsNamlu;
    
    [Header("Mermi İzi (Tracer) Ayarı")]
    public TrailRenderer tracerEffect;
    private PersistentTracer _tracer;

    private void Start()
    {
        _tracer = GetComponentInChildren<PersistentTracer>();

    }

    [PunRPC]
    public void RPC_PlayShootSource(byte _index)
    {
        shootSource.clip = shootSFX[_index];
        shootSource.Stop();
        shootSource.Play();
    }

    

    [PunRPC]
    public void RPC_NamluFlasi(int namluYeri)
    {
        Transform kullanilacakNokta = photonView.IsMine ? fpsNamlu[namluYeri] : tpsNamlu[namluYeri];

        if (namluFlasi && kullanilacakNokta)
        {
            GameObject flash = Instantiate(namluFlasi, kullanilacakNokta.position, kullanilacakNokta.rotation);
            flash.transform.SetParent(kullanilacakNokta);
            Destroy(flash, 0.1f);
        }
    }
    [PunRPC]
    public void RPC_MermiIzi(Vector3 hedefNokta, int index)
    {
        Transform namluDizisi = photonView.IsMine ? fpsNamlu[index] : tpsNamlu[index];
        
        if (tracerEffect && namluDizisi)
        {
            var tracer = Instantiate(tracerEffect, namluDizisi.position, Quaternion.identity);
            tracer.AddPosition(namluDizisi.position);
            tracer.transform.position = hedefNokta;
        }
    }

    public void playSmoke(int index)
    {        
        Transform kullanilacakNokta = photonView.IsMine ? fpsNamlu[index] : tpsNamlu[index];

        _tracer.SpawnTracer(kullanilacakNokta.position);
    }

    public void AnimasyonAdimTetikle()
    {

        if (adimSFX.Length > 0 && adimSource != null)
        {
            int rastgeleIndex = Random.Range(0, adimSFX.Length);
            adimSource.pitch = Random.Range(0.85f, 1.15f);

            AudioClip clip = adimSFX[rastgeleIndex];
            adimSource.PlayOneShot(clip);
        }
    }
    public void AnimDusmeTetikle()
    {
        int rastgeleIndex = Random.Range(0, dusmeSFX.Length);
        adimSource.pitch = Random.Range(0.85f, 1.15f);
        AudioClip clip = dusmeSFX[rastgeleIndex];
        adimSource.PlayOneShot(clip);
        Debug.Log("dustum");

    }
    public void AnimYukselmeTetikle()
    {
        int rastgeleIndex = Random.Range(0, ziplaSFX.Length);
        adimSource.pitch = Random.Range(0.85f, 1.15f);
        AudioClip clip = ziplaSFX[rastgeleIndex];
        adimSource.PlayOneShot(clip);
        Debug.Log("yukseldim");

    }
    public void ReloadSynceAnim(byte reloadIndex)
    {

        reloadSource.PlayOneShot(reloadSFX[reloadIndex]);
    }

}
