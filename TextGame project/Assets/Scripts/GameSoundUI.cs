using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSoundUI : MonoBehaviour
{
    public AudioClip buttonSelect;
    public AudioClip buttonPress;
    
    AudioSource audioSource;
    Button button;

    void Awake()
    {
        audioSource = GameObject.Find("SoundManager").GetComponent<AudioSource>();
        button = GetComponent<Button>();
    }
    void Update()
    {
        button.onClick.AddListener(SoundPlay);
    }

    private void SoundPlay()
    {
        audioSource.clip = buttonPress;
        audioSource.Play();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.clip = buttonSelect;
        audioSource.Play();
    }
}
