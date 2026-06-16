using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealt : HealtBase
{
    [Header("Oyuncu UI Elemanları")]
    public TextMeshProUGUI saglikYazi;
    public Image saglikIndikator;
    protected override void updateUI()
    {
        if (saglikIndikator == null || saglikYazi == null) return;
        saglikIndikator.fillAmount = (float)healt / maxHealt;
        saglikYazi.text = $"<b>{healt}/</b>{maxHealt}";
    }
    protected override void deathLogic()
    {
        if (photonView.IsMine)
        {
            LokalKDYonetim.Instance.OnDied();
            YonetimScript.Instance.YenidenDog();
            
        }
    }
}
