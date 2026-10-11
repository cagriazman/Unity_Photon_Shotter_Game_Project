using System.Collections;
using UnityEngine;

namespace Silah
{
    public class Shotgun_Silah: SilahTaban
    {
        protected override void AtesAnimasyonuOynat()
        {
            anim.SetTrigger("Fire");
        }

        protected override void Reload()
        {
            StartCoroutine(ShotgunTekTekReload());
        
            
        }

        IEnumerator ShotgunTekTekReload()
        {
            isReloading = true;
            anim.SetBool("IsReloading", true);
            

            int ihtiyac = WeaponStruct.sarjorKapasitesi - WeaponStruct.mevcutMermi;
            int toplamEklenecek = Mathf.Min(ihtiyac, WeaponStruct.yedekMermi);

            
            for (int i = 0; i < toplamEklenecek; i++)
            {
                anim.SetTrigger("Reload"); 
            
                yield return new WaitForSeconds(silahOzellikleriOS.reloadSure);

                 MermiMatematiginiYap(1);
                
            }

            anim.SetBool("IsReloading", false);
            isReloading = false;
        }
       

    }
}