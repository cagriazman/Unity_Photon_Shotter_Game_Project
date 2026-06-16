using UnityEngine;
using Photon.Pun;

public class NetworkAimSync : MonoBehaviourPun, IPunObservable
{
    [Header("Rig Target Objesi")]
    [Tooltip("Rig Builder'ın baktığı hedef obje (Target)")]
    public Transform rigTarget;

    

    [Header("Ayarlar")]
    public float rayMesafe = 100f;
    public LayerMask hitLayers;
    public float smoothSpeed = 15f;

    private Vector3 networkTargetPos;
    private Transform mainCameraTransform; 

    void Start()
    {
        if (photonView.IsMine)
        {
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
            
        }
    }

    void LateUpdate()
    {
        if (rigTarget == null) return;

        if (photonView.IsMine)
        {
            if (mainCameraTransform != null)
            {
                RaycastVeHedefiAyarla();
            }
        }
        else
        {
            rigTarget.position = Vector3.Lerp(rigTarget.position, networkTargetPos, Time.deltaTime * smoothSpeed);
        }
    }

    void RaycastVeHedefiAyarla()
    {
        Ray ray = new Ray(mainCameraTransform.position, mainCameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayMesafe, hitLayers))
        {
            rigTarget.position = hit.point;
        }
        else
        {
            rigTarget.position = mainCameraTransform.position + (mainCameraTransform.forward * rayMesafe);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigTarget.position);
        }
        else
        {
            networkTargetPos = (Vector3)stream.ReceiveNext();
        }
    }
}