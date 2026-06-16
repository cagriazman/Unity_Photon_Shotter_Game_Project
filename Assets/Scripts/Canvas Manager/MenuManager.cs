using Photon.Pun;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Menü Arayüzü")]
    public GameObject menuPanel; 

    private bool isMenuOpen = false;
    public GameObject bot;
    public Transform ctSpawn,tSpawn;
   private Transform newPos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen)
                ResumeGame();
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true); 
        isMenuOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false); 
        isMenuOpen = false;
        Cursor.lockState = CursorLockMode.Locked;       
        Cursor.visible = false;
        
    }
    
    public void Btn_addBot(int botTeamNumber)
    {
        int teamNumber = botTeamNumber;
        newPos =(teamNumber ==0) ? tSpawn : ctSpawn;
        object[] sendAttribute = new object[] { teamNumber };
        float randomPositionX = Random.Range(-3f, 3f);
        float randomPositionY = Random.Range(-3f, 3f);
        Vector3 newPosition = new  Vector3(newPos.position.x+randomPositionX,newPos.position.y,newPos.position.z+randomPositionY);
        PhotonNetwork.Instantiate(bot.name, newPosition , newPos.rotation,0,sendAttribute);
       
    }

    public void Btn_TakimSec()
    {
       
        YonetimScript.Instance.YenidenDog();
        ResumeGame();
    }

    public void Btn_Cikis()
    {
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); 
        #endif
    }
}