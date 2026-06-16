using UnityEngine;

public class Smg_Silah : SilahTaban
{
    protected override void AtesAnimasyonuOynat()
    {
        anim.SetTrigger("Fire");
    }
   
}
