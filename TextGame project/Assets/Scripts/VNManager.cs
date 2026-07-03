using System.Collections;
using System.Collections.Generic;
using ExcelDataReader;
using TMPro;
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
    public Image CharacterImage1;
    public Image CharacterImage2;
    public Image CharacterImage3;

    private string storyPath = Constants.STORY_PATH;
    private string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.DEFAULT_START_LINE;
    
    void Start()
    {
        LoadStoryFromFile(storyPath + defaultStoryFileName);
        DisplayNextLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextLine();
        }
    }

    void LoadStoryFromFile(string path)
    {
        storyData = ExcelReader.ReadExcel(path);
        if (storyData.Count == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }
    }

    void DisplayNextLine()
    {
        if (currentLine >= storyData.Count)
        {
            Debug.Log(Constants.END_OF_STORY);
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

        if (NotNullNorEmpty(data.character1Action))
        {
            UpdateCharacterImage(data.character1Action, data.character1ImageFileName,CharacterImage1);
        }
        if (NotNullNorEmpty(data.character2Action))
        {
            UpdateCharacterImage(data.character2Action, data.character2ImageFileName,CharacterImage2);
        }
        if (NotNullNorEmpty(data.character3Action))
        {
            UpdateCharacterImage(data.character3Action, data.character3ImageFileName,CharacterImage3);
        }
        currentLine++;
    }

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
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
    }

    void PlayBackgroundMusic(string musicFileName)
    {
        string musicPath = Constants.MUSIC_PATH + musicFileName;
        PlayAudio(musicPath, backgroundMusic, true);
    }

    void UpdateCharacterImage(string action, string imageFileName, Image characterImage)
    {
        if (action.StartsWith(Constants.characterActionAppearAt))
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            UpdateImage(imagePath, characterImage);
        }
        else if (action == Constants.characterActionDisappear)
        {
            characterImage.gameObject.SetActive(false);
        }
        else if (action.StartsWith(Constants.characterActionMoveTo))
        {
            
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
            if (audioSource == vocalAudio)
            {
                Debug.LogError(Constants.VOCAL_LOAD_FAILED + audioPath);
            }
            else if (audioSource == backgroundMusic)
            {
                Debug.LogError(Constants.MUSIC_LOAD_FAILED + audioPath);
            }
        }
    }
}
