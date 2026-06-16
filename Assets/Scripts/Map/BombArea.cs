using System;
using Unity.VisualScripting;
using UnityEngine;

public class BombArea : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           OyuncuIcın gelen= other.gameObject.GetComponent<OyuncuIcın>();
           gelen.canPlant = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OyuncuIcın gelen= other.gameObject.GetComponent<OyuncuIcın>();
            gelen.canPlant = false;
        }
    }

    
}
