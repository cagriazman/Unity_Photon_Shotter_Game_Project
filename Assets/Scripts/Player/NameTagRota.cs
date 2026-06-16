using UnityEngine;

public class NameTagRota : MonoBehaviour
{


    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
