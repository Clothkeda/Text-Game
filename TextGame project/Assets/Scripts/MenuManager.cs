using UnityEngine;
using UnityEngine.UI;
    

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public Button startButton;
    public Button continueButton;
    public Button loadButton;
    public Button settingsButton;
    public Button quitButton;
    
    private bool hasStarted = false;
    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        MenuButtonsAddListener();
    }

    void MenuButtonsAddListener()
    {
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        loadButton.onClick.AddListener(LoadGame);
        settingsButton.onClick.AddListener(ShowSettingPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        hasStarted = true;
        VNManager. Instance.StartGame(Constants.NEW_STORY_FILE_NAME, Constants.DEFAULT_START_LINE);
        ShowGamePanel();
    }
    private void ContinueGame()
    {
        if (hasStarted)
        {
            ShowGamePanel();
        }
    }
    private void LoadGame()
    {
        VNManager.Instance.ShowLoadPanel(ShowGamePanel);
    }
    private void ShowGamePanel()
    {
        menuPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
    }
    private void ShowSettingPanel()
    {
        SettingManager.Instance.ShowSettingPanel();
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
