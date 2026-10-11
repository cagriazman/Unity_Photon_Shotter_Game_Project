using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class Sniper_Silah : SilahTaban
{
    [Header("Cinemachine Ayarları")]
    public CinemachineVirtualCamera vCam;
    public GameObject scopeOverlay;
    
    [Header("Zoom Ayarları")]
    public float scopeFOV = 15f;
    public float zoomHizi = 15f; 
    
    private float defaultFOV;
    private bool sonAdsDurumu; 
    
    protected override void Start() {
        base.Start();
    
        if (vCam == null) {
            vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        }
        
        if (vCam != null) {
            defaultFOV = vCam.m_Lens.FieldOfView;
        }

        if (scopeOverlay) scopeOverlay.SetActive(false);
    }

    protected override void Update()
    {

        base.Update();
        
        float hedefFOV = ads ? scopeFOV : defaultFOV;
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, hedefFOV, Time.deltaTime * zoomHizi);
        
        if (ads != sonAdsDurumu)
        {
            if (scopeOverlay) scopeOverlay.SetActive(ads);
            
            sonAdsDurumu = ads; 
        }
    }
    
    protected override void AtesAnimasyonuOynat()
    {
        anim.SetTrigger("Fire");
    }
    
}