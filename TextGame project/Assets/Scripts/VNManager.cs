using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using ExcelDataReader;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakingContent;
    public TypewriterEffect typewriterEffect;
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

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.DEFAULT_START_LINE;
    
    void Start()
    {
        InitializeAndLoadStory(defaultStoryFileName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsHittingBottomButtons())
            {
                DisplayNextLine();
            }
        }
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
        avatarImage.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        //pageImage.gameObject.SetActive(false);
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        characterImage3.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        autoButton.onClick.AddListener(OnAutoButtonClick);
    }

    void LoadStoryFromFile(string fileName)
    {
        var path = storyPath + fileName + excelFileExtension;
        storyData = ExcelReader.ReadExcel(path);
        if (storyData.Count == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }
    }

    void DisplayNextLine()
    {
        if (currentLine == storyData.Count - 1)
        {
            if(storyData[currentLine].speakerName == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
                return;
            }

            if (storyData[currentLine].speakerName == Constants.CHOICE)
            {
                ShowChoices();
                return;
            }
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
        speakingContent.text = data.speakingContent;
        typewriterEffect.StartTyping(speakingContent.text);
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

    void UpdateAvatarImage(string imageFileName)
    {
        var imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);
    }

    void PlayVocalAudio(string audioFileName)
    {
        string audioPath = Constants.VOCAL_PATH + audioFileName;
        PlayAudio(audioPath, vocalAudio, false);
    }

    void UpdateBackgroundImage(string imageFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + imageFileName;
        UpdateImage(imagePath, backgroundImage);
        backgroundImage.DOFade(1, Constants.DURATION_TIME).From(0);
    }

    void PlayBackgroundMusic(string musicFileName)
    {
        string musicPath = Constants.MUSIC_PATH + musicFileName;
        PlayAudio(musicPath, backgroundMusic, true);
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

    void PlayAudio(string audioPath, AudioSource audioSource, bool isLoop)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.loop = isLoop;
        }
        else
        {
            Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
        }
    }

    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            null
            );
    }
    private bool isAutoPlay = false;

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

    void UpdateButtonImage(string imageFileName, Button button)
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;
        UpdateImage(imagePath, button.image);
    }

    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typewriterEffect.IsTyping())
            {
                DisplayNextLine();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_WAITING_SECONDS);
        }
    }
}
