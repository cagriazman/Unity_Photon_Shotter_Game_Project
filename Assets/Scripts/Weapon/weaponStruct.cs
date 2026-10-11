using UnityEngine;

[System.Serializable] 
public struct weaponStruct
{
    public int silahTuru;
    public int yedekMermi;
    public int mevcutMermi;
    public int sarjorKapasitesi;
    public float siradakiAtisZamani ;
    
    public weaponStruct(SilahOzellikleriOS os)
    {
        yedekMermi = os.yedekMermi;
        mevcutMermi = os.mevcutMermi; 
        sarjorKapasitesi = os.sarjorKapasitesi;
        siradakiAtisZamani = 0;
        silahTuru = os.silahTur;
    }
}