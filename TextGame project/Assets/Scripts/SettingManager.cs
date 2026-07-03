using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("音量滑块")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    
    [Header("音频混音器")]
    public AudioMixer audioMixer;

    // 音量存档Key常量（统一写在本脚本，不用外部Constants类）
    public const string MASTER_VOLUME_KEY = "MasterVolume";
    public const string MUSIC_VOLUME_KEY = "MusicVolume";
    public const float DEFAULT_VOLUME = 1.0f;
    // 混音器最小静音分贝
    private const float MIN_DB = -80f;

    void Start()
    {
        // 1. 读取本地存档赋值给滑块
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);
        
        masterVolumeSlider.value = masterVol;
        musicVolumeSlider.value = musicVol;

        // 2. 只注册一次滑块监听（移走Update里的重复注册）
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        // 3. 初始化混音器音量
        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
    }

    // 主音量设置 + 本地保存
    void SetMasterVolume(float sliderValue)
    {
        float db = SliderValueToDecibel(sliderValue);
        audioMixer.SetFloat(MASTER_VOLUME_KEY, db);
        
        // 保存到本地存档
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, sliderValue);
        PlayerPrefs.Save();
    }

    // 音乐音量设置 + 本地保存
    void SetMusicVolume(float sliderValue)
    {
        float db = SliderValueToDecibel(sliderValue);
        audioMixer.SetFloat(MUSIC_VOLUME_KEY, db);
        
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, sliderValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 滑块0~1数值 转 AudioMixer 分贝(-80 ~ 0db)
    /// </summary>
    private float SliderValueToDecibel(float sliderValue)
    {
        // 滑块为0时直接返回静音值
        if (sliderValue <= 0.0001f)
            return MIN_DB;
        
        // 对数转换匹配人耳听觉
        return Mathf.Log10(sliderValue) * 20;
    }

    // 销毁时移除监听，防止内存泄漏
    private void OnDestroy()
    {
        if(masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if(musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
    }

    // 移除无用空Update，不需要每帧执行任何逻辑
    // void Update() 已删除
    
    // 冗余无用函数全部删除 Initialization / AddListener / InitializeVolume
}
