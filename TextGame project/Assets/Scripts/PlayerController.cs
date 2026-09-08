using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Animator ani;
    public Rigidbody2D rBody;
    public float lastH;
    public float lastV;

    [Header("交互设置")]
    public bool isInteracting = false;

    [Header("游戏状态")]
    public bool isGameActive = false;

    [Header("通关设置")]
    public string gameSceneName = "Game";

    private HashSet<string> interactedObjects = new HashSet<string>();

    void Start()
    {
        ani = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isInteracting && VNManager.Instance != null
                       && !VNManager.Instance.dialogueBox.activeSelf)
        {
            isInteracting = false;
        }

        if (isInteracting)
        {
            if (rBody != null) rBody.linearVelocity = Vector2.zero;
            if (ani != null) ani.SetFloat("Speed", 0);
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 dir = Vector2.zero;

        if (horizontal != 0)
        {
            dir = new Vector2(horizontal, 0);
            lastH = horizontal;
            lastV = 0;
        }
        else if (vertical != 0)
        {
            dir = new Vector2(0, vertical);
            lastH = 0;
            lastV = vertical;
        }

        if (ani != null)
        {
            ani.SetFloat("Horizontal", lastH);
            ani.SetFloat("Vertical", lastV);
            ani.SetFloat("Speed", dir.magnitude);
        }

        if (rBody != null)
        {
            rBody.linearVelocity = dir * 2f;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("按下Ctrl键");
            EndGame();
        }
    }

    public void RegisterInteract(string objectKey)
    {
        if (!string.IsNullOrEmpty(objectKey))
        {
            interactedObjects.Add(objectKey);
        }
    }

    public bool HasInteracted(string objectKey)
    {
        return interactedObjects.Contains(objectKey);
    }

    public bool HasInteractedAll(string[] requiredObjects)
    {
        foreach (string obj in requiredObjects)
        {
            if (!interactedObjects.Contains(obj))
            {
                return false;
            }
        }
        return true;
    }

    public void EndGame()
    {
        isGameActive = false;
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(Constants.SAMPLE_SCENE);
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnGameSceneLoaded;

        if (scene.name == Constants.SAMPLE_SCENE && VNManager.Instance != null)
        {
            // 退出小游戏模式，直接加载名为"11"的新剧情表格并开始播放
            VNManager.Instance.BackTextGame("11");
        }
    }
}
