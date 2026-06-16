using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class camRecoil : MonoBehaviour
{
    [SerializeField] private Vector3 randomValue;
    Vector3 originalPos;
    private Vector3 right;
    [SerializeField]float rastgeleX;
    [SerializeField] private float rastgeleY;
    Quaternion orginalRot;

    public float recoveryTimerAcc =0.1f;
    public float returnDelay = 0.2f;
    private Coroutine recoveryCoroutine;
   
    private void Awake()
    {
        originalPos = transform.localPosition;
        orginalRot = transform.localRotation;
    }

    public void Recoil(float time, float power,float downcharRotationSpeed,float upcharRotationSpeed)
    {
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
        }
        StartCoroutine(RecoilCoroutine(time,power,downcharRotationSpeed,upcharRotationSpeed));
    }

    IEnumerator RecoilCoroutine(float time, float power,float downcharRotationSpeed,float upcharRotationSpeed)
    {
        float timer = 0f;

        while (timer < time)
        {
            
            rastgeleX = Random.Range(-1f, 1f);
            rastgeleY = Random.Range(0f, 2f);
           
            Vector3 randomDotForRot = Vector3.up * rastgeleY;
            Vector3 randomDotForPos=Vector3.right * rastgeleX + randomDotForRot;
            Quaternion targetRotation = Quaternion.LookRotation(randomDotForRot);
            
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, upcharRotationSpeed * Time.deltaTime);
            transform.localPosition = originalPos + randomDotForPos * power;

            timer += Time.deltaTime;
            yield return null; 
        }
        
        recoveryCoroutine = StartCoroutine(GeriToplanmaCoroutine(downcharRotationSpeed));
    }
    
    IEnumerator GeriToplanmaCoroutine(float downcharRotationSpeed)
    {
        yield return new WaitForSeconds(returnDelay);

        float recoveryTimer = 0f;

        while (recoveryTimer < 1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, orginalRot, downcharRotationSpeed * Time.deltaTime);
            //transform.localRotation = orginalRot;
            transform.localPosition = originalPos;
            recoveryTimer += Time.deltaTime * recoveryTimerAcc; 

            yield return null;
        }

        
    }
}




