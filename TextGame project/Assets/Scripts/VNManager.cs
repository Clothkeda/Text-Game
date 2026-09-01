using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    #region Variables
    public GameObject gamePanel;
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerName;
    public TypewriterEffect typewriterEffect;
    public ScreenShotter screenShotter;
    
    public Image avatarImage;
    public AudioSource vocalAudio;
    public Image backgroundImage;
    public AudioSource backgroundMusic;
    public Image pageImage;
    public Image characterImage1;
    public Image characterImage2;
    public Image characterImage3;
    
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    
    public GameObject bottomButtons;
    public Button autoButton;
    public Button skipButton;
    public Button saveButton;
    public Button loadButton;
    public Button historyButton; 
    public Button settingsButton; 
    public Button homeButton;
    public Button closeButton;

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;

    private string saveFolderPath;
    private byte[] screenshotData;
    private string currentSpeakingContent;
    
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine;
    private string currentStoryFileName;
    private float currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED;
    
    private bool isAutoPlay = false;
    private bool isSkip = false;
    private int maxReachedLineIndex = 0;
    private Dictionary<string, int> globalMaxReachedLineIndices = new Dictionary<string, int>();
    public static VNManager Instance { get; private set; }
    #endregion
    #region Lifecycle
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
        InitializeSaveFilePath();
        bottomButtonsAddListener();
    }
    void Update()
    {
        if (!MenuManager.Instance.menuPanel.activeSelf &&
            !SaveLoadManager.Instance.saveLoadPanel.activeSelf &&
            gamePanel.activeInHierarchy &&  Input.GetMouseButtonDown(0))
        {
            if (!dialogueBox.activeSelf)
            {
                OpenUI();
            }
            else if (!IsHittingBottomButtons())
            {
                DisplayNextLine();
            }
        }
    }
    #endregion
    #region Initialization
    void InitializeSaveFilePath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }
    void bottomButtonsAddListener()
    {
        autoButton.onClick.AddListener(OnAutoButtonClick);
        skipButton.onClick.AddListener(OnSkipButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
        
        homeButton.onClick.AddListener(OnHomeButtonClick);
        homeButton.onClick.AddListener(OnCloseButtonClick);
    }
    public void StartGame()
    {
        InitializeAndLoadStory(defaultStoryFileName);
    }
    void InitializeAndLoadStory(string fileName)
    {
        Initialize();
        LoadStoryFromFile(fileName);
        DisplayNextLine();
    }
    void Initialize()
    {
        currentLine = Constants.DEFAULT_START_LINE;
        //pageImage.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        backgroundMusic.gameObject.SetActive(false);
        
        avatarImage.gameObject.SetActive(false);
        vocalAudio.gameObject.SetActive(false);
        
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        characterImage3.gameObject.SetActive(false);
        
        choicePanel.SetActive(false);
    }
    void LoadStoryFromFile(string fileName)
    {
        currentStoryFileName = fileName;
        var path = storyPath + fileName + excelFileExtension;
        storyData = ExcelReader.ReadExcel(path);
        if (storyData.Count == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }
        if(globalMaxReachedLineIndices.ContainsKey(currentStoryFileName))
        {
            maxReachedLineIndex = globalMaxReachedLineIndices[currentStoryFileName];
        }
        else
        {
            maxReachedLineIndex = 0;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }
    }
    #endregion
    #region Display
    void DisplayNextLine()
    {
        if (currentLine > maxReachedLineIndex)
        {
            maxReachedLineIndex = currentLine;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }

        if (currentLine >= storyData.Count - 1)
        {
            if (isAutoPlay)
            {
                isAutoPlay = false;
                UpdateButtonImage(Constants.AUTO_OFF, autoButton);
            }

            if (storyData[currentLine].speakerName == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
            }

            if (storyData[currentLine].speakerName == Constants.CHOICE)
            {
                ShowChoices();
            }
            return;
        }
        if (typewriterEffect.IsTyping())
        {
            typewriterEffect.CompleteLine();
        }
        else
        {
            DisplayThisLine();
        }
    }

    void DisplayThisLine()
    {
        var data = storyData[currentLine];
        speakerName.text = data.speakerName;
        currentSpeakingContent = data.speakingContent;
        typewriterEffect.StartTyping(currentSpeakingContent, currentTypingSpeed);
        if (NotNullNorEmpty(data.avatarImageFileName))
        {
            UpdateAvatarImage(data.avatarImageFileName);
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }
        if (NotNullNorEmpty(data.vocalAudioFileName))
        {
            PlayVocalAudio(data.vocalAudioFileName);
        }
        if (NotNullNorEmpty(data.backgroundImageFileName))
        {
            UpdateBackgroundImage(data.backgroundImageFileName);
        }
        if (NotNullNorEmpty(data.backgroundMusicFileName))
        {
            PlayBackgroundMusic(data.backgroundMusicFileName);
        }
        if (NotNullNorEmpty(data.pageAction))
        {
            UpdatePageImage(data.pageAction, data.pageImageFileName,pageImage);
        }
        if (NotNullNorEmpty(data.character1Action))
        {
            UpdateCharacterImage(data.character1Action, data.character1ImageFileName,characterImage1, data.coordinateX1);
        }
        if (NotNullNorEmpty(data.character2Action))
        {
            UpdateCharacterImage(data.character2Action, data.character2ImageFileName,characterImage2, data.coordinateX2);
        }
        if (NotNullNorEmpty(data.character3Action))
        {
            UpdateCharacterImage(data.character3Action, data.character3ImageFileName,characterImage3, data.coordinateX3);
        }
        currentLine++;
    }

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }
    #endregion
    #region Choices
    void ShowChoices()
    {
        var data = storyData[currentLine];
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();
        choicePanel.SetActive(true);
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = data.speakingContent;
        choiceButton1.onClick.AddListener(() => InitializeAndLoadStory(data.avatarImageFileName));
        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = data.vocalAudioFileName;
        choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(data.backgroundImageFileName));
    }
    #endregion
    #region Audios
    void PlayVocalAudio(string audioFileName)
        {
            string audioPath = Constants.VOCAL_PATH + audioFileName;
            PlayAudio(audioPath, vocalAudio, false);
        }
    void PlayBackgroundMusic(string musicFileName)
        {
            string musicPath = Constants.MUSIC_PATH + musicFileName;
            PlayAudio(musicPath, backgroundMusic, true);
        }
    void PlayAudio(string audioPath, AudioSource audioSource, bool isLoop)
        {
            AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.gameObject.SetActive(true);
                audioSource.Play();
                audioSource.loop = isLoop;
            }
            else
            {
                Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
            }
        }
    #endregion
    #region Images
    void UpdateAvatarImage(string imageFileName)
    {
        var imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);
    }
    void UpdateBackgroundImage(string imageFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + imageFileName;
        UpdateImage(imagePath, backgroundImage);
        backgroundImage.DOFade(1, Constants.DURATION_TIME).From(0);
    }
    void UpdatePageImage(string action, string imageFileName, Image pageImage)
    {
        if (action.StartsWith(Constants.APPEAR_AT))
        {
            string imagePath = Constants.PAGE_PATH + imageFileName;
            UpdateImage(imagePath, pageImage);
            pageImage.DOFade(1, Constants.DURATION_TIME).From(0);
        }
        else if (action == Constants.DISAPPEAR)
        {
            pageImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => pageImage.gameObject.SetActive(false));
        }
    }
    void UpdateCharacterImage(string action, string imageFileName, Image characterImage, string x)
    {
        if (action.StartsWith(Constants.APPEAR_AT))
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            if (NotNullNorEmpty(x))
            {
                UpdateImage(imagePath, characterImage);
                var newPosition = new Vector2(float.Parse(x), characterImage.rectTransform.anchoredPosition.y);
                characterImage.rectTransform.anchoredPosition = newPosition;
                characterImage.DOFade(1, Constants.DURATION_TIME).From(0);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
        else if (action == Constants.DISAPPEAR)
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => characterImage.gameObject.SetActive(false));
        }
        else if (action.StartsWith(Constants.MOVE_TO))
        {
            if (NotNullNorEmpty(x))
            {
                characterImage.rectTransform.DOAnchorPosX(float.Parse(x), Constants.DURATION_TIME);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
    }
    void UpdateButtonImage(string imageFileName, Button button)
        {
            string imagePath = Constants.BUTTON_PATH + imageFileName;
            UpdateImage(imagePath, button.image);
        }
    void UpdateImage(string imagePath, Image image)
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }
    #endregion
    #region Buttons
    #region Bottom
    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            Camera.main
            );
    }
    #endregion
    #region Auto
    void OnAutoButtonClick()
    {
        isAutoPlay =  !isAutoPlay;
        Debug.Log("isAutoPlay");
        UpdateButtonImage((isAutoPlay ? Constants.AUTO_ON : Constants.AUTO_OFF), autoButton);
        if (isAutoPlay)
        {
            StartCoroutine(StartAutoPlay());
        }
    }
    private IEnumerator StartAutoPlay()
        {
            while (isAutoPlay)
            {
                if (!typewriterEffect.IsTyping())
                {
                    DisplayNextLine();
                }
                yield return new WaitForSeconds(Constants.DEFAULT_AUTO_WAITING_SECONDS);
            }
        }
    #endregion
    #region Skip
    void OnSkipButtonClick()
    {
        if (!isSkip && CanSkip())
        {
            StartSkip();
        }
        else if (isSkip)
        {
            StopCoroutine(SkipToMaxReachedLine());
            EndSkip();
        }
    }
    bool CanSkip()
        {
            return currentLine < maxReachedLineIndex;
        }
    void StartSkip()
        {
            isSkip = true;
            UpdateButtonImage(Constants.SKIP_ON, skipButton);
            currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
            StartCoroutine(SkipToMaxReachedLine());
        }
    void EndSkip()
        {
            isSkip = false;
            currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED;
            UpdateButtonImage(Constants.SKIP_OFF, skipButton);
        }
    private IEnumerator SkipToMaxReachedLine()
        {
            while (isSkip)
            {
                if (CanSkip())
                {
                    DisplayThisLine();
                }
                else
                {
                    EndSkip();
                }
                yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);
            }
        }
    #endregion
    #region Save
    void OnSaveButtonClick()
    {
        CloseUI();
        Texture2D screenshot = screenShotter.CaptureScreenshot();
        screenshotData = screenshot.EncodeToPNG();
        SaveLoadManager.Instance.ShowSavePanel(SaveGame);
        OpenUI();
    }

    void SaveGame(int slotIndex)
    {
        var saveData = new SaveData
        {
            currentSpeakingContent = currentSpeakingContent,
            screenshotData = screenshotData
        };
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    public class SaveData
    {
        public string currentSpeakingContent;
        public byte[] screenshotData;
    }
    #endregion
    #region Load
    void OnLoadButtonClick()
    {
        SaveLoadManager.Instance.ShowLoadPanel(LoadGame);
    }
    void LoadGame(int slotIndex)
    {
        
    }
    #region Home
    void OnHomeButtonClick()
    {
        gamePanel.SetActive(false);
        MenuManager.Instance.menuPanel.SetActive(true);
    }
    #endregion
    #region Close
    void OnCloseButtonClick()
    {
        CloseUI();
    }
    void OpenUI()
    {
        dialogueBox.SetActive(true);
        bottomButtons.SetActive(true);
    }
    void CloseUI()
    {
        dialogueBox.SetActive(false);
        bottomButtons.SetActive(false);
    }
    #endregion
    #endregion
    #endregion
}
