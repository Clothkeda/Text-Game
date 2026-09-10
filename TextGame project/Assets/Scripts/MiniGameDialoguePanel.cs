using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniGameDialoguePanel : MonoBehaviour
{
    public static MiniGameDialoguePanel Instance;

    [Header("只需要拖拽面板和预制体！不要拖场景内文本")]
    public GameObject dialoguePanel;
    public Text speakerPrefab;
    public Text contentPrefab;

    private DialoguePoint currentDialogue;
    private System.Action<DialoguePoint> finishCallback;
    private int index;
    private bool isTyping;

    // 动态生成的文本实例
    private Text spawnedSpeaker;
    private Text spawnedContent;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        dialoguePanel.SetActive(false);
    }

    // Player调用这个函数，打开面板并传入对话数据
    public void OpenDialogue(DialoguePoint dialogueData, System.Action<DialoguePoint> callback)
    {
        // 先清理上一轮残留文本
        DestroyLastTextObjects();

        currentDialogue = dialogueData;
        finishCallback = callback;
        index = 0;
        dialoguePanel.SetActive(true);

        // 实例化 speaker 和 content，作为dialoguePanel的子物体
        spawnedSpeaker = Instantiate(speakerPrefab, dialoguePanel.transform);
        spawnedContent = Instantiate(contentPrefab, dialoguePanel.transform);

        ShowCurrentSentence();
    }

    void ShowCurrentSentence()
    {
        if(index >= currentDialogue.sentences.Count)
        {
            CloseDialogue();
            return;
        }
        var sen = currentDialogue.sentences[index];
        spawnedSpeaker.text = sen.speakerName;
        StartCoroutine(TypeText(sen.content));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        spawnedContent.text = "";
        foreach(var c in text)
        {
            spawnedContent.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
    }

    void Update()
    {
        if(!dialoguePanel.activeSelf) return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isTyping)
            {
                StopAllCoroutines();
                spawnedContent.text = currentDialogue.sentences[index].content;
                isTyping = false;
            }
            else
            {
                index++;
                ShowCurrentSentence();
            }
        }
    }

    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        DestroyLastTextObjects();

        // 回调给Player，标记对话完成
        finishCallback?.Invoke(currentDialogue);
        currentDialogue = null;
        finishCallback = null;
    }

    void DestroyLastTextObjects()
    {
        if(spawnedSpeaker != null) Destroy(spawnedSpeaker.gameObject);
        if(spawnedContent != null) Destroy(spawnedContent.gameObject);
        spawnedSpeaker = null;
        spawnedContent = null;
    }

    private void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }
}