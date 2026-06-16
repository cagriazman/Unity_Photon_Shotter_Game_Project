using UnityEngine;

public class SarjorDegistirmeScript : MonoBehaviour
{
    public AudioSource SarjorAudioSource;
    public AudioClip girmeSesi;
    public AudioClip cikarmaSesi;
    public AudioClip cekmeSesi;

    public void CalGirmeSesi()
    {
        SarjorAudioSource.PlayOneShot(girmeSesi);
    }
    public void CalCikmaSesi()
    {
        SarjorAudioSource.PlayOneShot(cikarmaSesi);
    }
    public void CalCekmeSesi()
    {
        SarjorAudioSource.PlayOneShot(cekmeSesi);
    }
}
