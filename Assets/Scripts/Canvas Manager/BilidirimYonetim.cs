using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class BilidirimYonetim : MonoBehaviourPun
{
    public static BilidirimYonetim Instance;

    [Header("UI")]
    public GameObject bildirimItemPrefab;
    public Transform bildirimItemParent;

    void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void RPC_bildirimVer(string _vuran, string _olen)
    {
        GameObject item = Instantiate(bildirimItemPrefab, bildirimItemParent);
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _vuran + " -Vurdu- " + _olen;
        StartCoroutine(GecikmeAktif(item.transform.GetChild(0).gameObject));

        Destroy(item, 2f);
    }
    IEnumerator GecikmeAktif(GameObject itemText)
    {
        itemText.gameObject.SetActive(false);
        yield return null;

        itemText.gameObject.SetActive(true);
    }

}
