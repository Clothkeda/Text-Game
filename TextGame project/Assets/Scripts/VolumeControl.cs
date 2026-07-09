using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VolumeControl : MonoBehaviour
{
    Button back;
    public GameObject settingUI;

    private void Awake()
    {
        back=transform. GetChild(3).GetComponent<Button>();
        
        back.onClick.AddListener(Back);
    }
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    void Back()
    {
        settingUI.gameObject.SetActive(false);
    }
}
