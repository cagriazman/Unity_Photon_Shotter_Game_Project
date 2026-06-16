using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;

public class AnimasyonSenkronizasyon : MonoBehaviourPun, IPunObservable 
{
    public Animator animator;
    public TwoBoneIKConstraint IK;

    [Range(-2, 2)]
    public float Horizontal;
    [Range(-2, 2)]
    public float Vertical;
    [Range(-180, 180)]
    public float HorizontalForMouse;

    private float targetHorizontal;
    private float targetVertical;
    private float targetHorizontalForMouse;
    [SerializeField] private float targetIKvalue;
    public float IKvalue;

    public float smoothSpeed = 10f;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Horizontal);
            stream.SendNext(Vertical);
            stream.SendNext(HorizontalForMouse);
            stream.SendNext(IKvalue);

        }
        else
        {
            targetHorizontal = (float)stream.ReceiveNext();
            targetVertical = (float)stream.ReceiveNext();
            targetHorizontalForMouse = (float)stream.ReceiveNext();
            targetIKvalue = (float)stream.ReceiveNext();


        }
    }

    [PunRPC]
    public void RPC_ZiplaAnim()
    {
        animator.SetTrigger("Zipla");
    }

    [PunRPC]
    public void RPC_SarjorDegis()
    {
        animator.SetTrigger("Reload");
    }

    [PunRPC]
    public void RPC_Crouch(bool value)
    {
        animator.SetBool("Crouch",value);
    }


    void Update()
    {
        if (photonView.IsMine)
        {
            animator.SetFloat("DikeyYon", Vertical);
            animator.SetFloat("YatayYon", Horizontal);
            animator.SetFloat("FareIcinYtyYn", HorizontalForMouse);

        }
        else
        {
            Horizontal = Mathf.Lerp(Horizontal, targetHorizontal, Time.deltaTime * smoothSpeed);
            Vertical = Mathf.Lerp(Vertical, targetVertical, Time.deltaTime * smoothSpeed);
            HorizontalForMouse = Mathf.Lerp(HorizontalForMouse, targetHorizontalForMouse, Time.deltaTime * smoothSpeed);


            animator.SetFloat("DikeyYon", Vertical);
            animator.SetFloat("YatayYon", Horizontal);
            animator.SetFloat("FareIcinYtyYn", HorizontalForMouse);
            if (IK != null)
            {
                IKvalue = Mathf.Lerp(IKvalue, targetIKvalue, Time.deltaTime * smoothSpeed);
                IK.weight = IKvalue;
            }

        }
    }
}