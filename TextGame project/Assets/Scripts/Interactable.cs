using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [Header("交互设置")]
    public string objectKey = "";
    public string interactText = "按 F 键进行对话";
    public string storyFileName = "";
    public int startLine = 0;

    [Header("通关点设置")]
    public bool isEndPoint = false;
    public string[] requiredObjects;
    public string notReadyText = "还有未完成的交互";

    [Header("UI引用")]
    public TextMeshProUGUI interactUIText;

    private bool isPlayerInRange = false;
    private PlayerController player;

    void Awake()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.GetComponent<PlayerController>();
            ShowUI();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HideUI();
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (isEndPoint)
        {
            if (player != null && player.HasInteractedAll(requiredObjects))
            {
                HideUI();
                player.EndGame();
            }
            else
            {
                if (interactUIText != null)
                {
                    interactUIText.text = notReadyText;
                }
            }
            return;
        }

        HideUI();
        if (player != null)
        {
            player.isInteracting = true;
            player.RegisterInteract(objectKey);
        }
        if (VNManager.Instance != null && !string.IsNullOrEmpty(storyFileName))
        {
            VNManager.Instance.gamePanel.SetActive(true);
            VNManager.Instance.StartGame(storyFileName, startLine);
        }
    }

    void ShowUI()
    {
        if (interactUIText != null)
        {
            interactUIText.text = interactText;
            interactUIText.gameObject.SetActive(true);
        }
    }

    void HideUI()
    {
        if (interactUIText != null)
        {
            interactUIText.gameObject.SetActive(false);
        }
    }
}
