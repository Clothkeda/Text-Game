using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    Button againBtn;
    Button quitBtn;
    Button setting;
    public GameObject VolumeUI;

    private void Awake()
    {
        againBtn=transform. GetChild(0).GetComponent<Button>();
        setting=transform. GetChild(1).GetComponent<Button>();
        quitBtn=transform. GetChild(2). GetComponent<Button>() ;
        
        againBtn.onClick.AddListener(PlayGame);
        setting.onClick.AddListener(Setting);
        quitBtn. onClick. AddListener (QuitGame);
    }
    void Start()
    {
        VolumeUI.gameObject.SetActive(false);
    }
    
    void Update()
    {
        
    }

    void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log(" 退 出 游 戏 ");
    }

    void Setting()
    {
        VolumeUI.gameObject.SetActive(true);
    }
}