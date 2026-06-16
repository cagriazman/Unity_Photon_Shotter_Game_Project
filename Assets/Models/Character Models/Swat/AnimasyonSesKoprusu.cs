using UnityEngine;
using Photon.Pun;

public class AnimasyonSesKoprusu : MonoBehaviour
{
    private PhotonOyucuSesYonetim anaYonetici;
    private CharacterController cc;

    void Awake()
    {

        anaYonetici = GetComponentInParent<PhotonOyucuSesYonetim>();
        cc = GetComponentInParent<CharacterController>();
    }
//model animatorcontrollerdan içinden animClipden gelen eventler
    public void AnimasyonAdimTetikle()
    {

        if (cc == null || anaYonetici == null) return;

        if (cc.isGrounded && cc.velocity.sqrMagnitude > 0.1f)
        {
            anaYonetici.AnimasyonAdimTetikle();
        }
    }//model animatorcontrollerdan içinden animClipden gelen eventler
    public void AnimDusmeTetikle()
    {
        anaYonetici.AnimDusmeTetikle();
        Debug.Log("dustum");

    }//model animatorcontrollerdan içinden animClipden gelen eventler
    public void AnimYukselmeTetikle()
    {
        anaYonetici.AnimYukselmeTetikle();
        Debug.Log("yukseldim");
    }
    public void ReloadAnimMagCikart()
    {
        anaYonetici.ReloadSynceAnim(0);
    }
    public void ReloadMagTak()
    {
        if (anaYonetici != null)
            anaYonetici.ReloadSynceAnim(1);
    }
}