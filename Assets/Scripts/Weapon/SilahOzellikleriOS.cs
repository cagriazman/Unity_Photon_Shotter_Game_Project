using UnityEngine;

[CreateAssetMenu(fileName = "SilahOzellikleri",menuName = "Ozellik/SilahOzellikleri")]

    public class SilahOzellikleriOS:ScriptableObject
    {
        [Header("Silah Özellikleri")] 
        public int silahTur =0;
        public float atesHizi = 16f;
        public int vurusHasari = 25;
        public float menzil = 100f;
        public float sacilma = 0.05f;
        public int sacmaSayisi = 1;
        public float recoilPower = 0.008f;
        public float recoilTime = 0.04f;
        
            
        [Header("Mermi Bilgileri")]
        public int yedekMermi = 90;
        public int mevcutMermi = 30;
        public int sarjorKapasitesi = 30;
        
        
        public float reloadSure = 1.6f;
        public float siradakiAtisZamani =0;
        public float mevcutSacilma;
        public float upcharRotationSpeed = 0.2f;
        public float downcharRotationSpeed = 10f;
        
    }
