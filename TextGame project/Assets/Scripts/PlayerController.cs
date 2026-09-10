using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueSentence
{
    public string speakerName;
    public string content;
}

[System.Serializable]
public class DialoguePoint
{
    public string pointName;
    public List<DialogueSentence> sentences;
    public bool isCompleted;
}

public class PlayerController : MonoBehaviour
{
    public Animator ani;
    public Rigidbody2D rBody;
    public float lastH;
    public float lastV;
    public GameObject tText;

    [Header("交互设置")]
    public bool isInteracting = false;
    public bool isGameActive = true;

    [Header("对话点")]
    public List<DialoguePoint> dialoguePoints = new List<DialoguePoint>();
    private DialoguePoint currentActiveDialoguePoint;

    // 终点触发器名字，在Hierarchy里终点碰撞物体名字必须和这个完全一样
    public string endTriggerName = "EndTrigger";

    void Start()
    {
        ani = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
        if (tText != null) tText.SetActive(false);
    }

    void Update()
    {
        if (isInteracting)
        {
            rBody.linearVelocity = Vector2.zero;
            ani.SetFloat("Speed",0);
            return;
        }

        // 玩家移动逻辑
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 dir = Vector2.zero;
        if(h != 0)
        {
            dir = new Vector2(h,0);
            lastH = h; lastV = 0;
        }
        else if(v !=0)
        {
            dir = new Vector2(0,v);
            lastH =0; lastV =v;
        }
        ani.SetFloat("Horizontal",lastH);
        ani.SetFloat("Vertical",lastV);
        ani.SetFloat("Speed", dir.magnitude);
        rBody.linearVelocity = dir * 2f;

        // ========= T键触发对话 =========
        if (Input.GetKeyDown(KeyCode.T))
        {
            try
            {
                if(currentActiveDialoguePoint == null)
                {
                    Debug.Log("附近没有对话点");
                    return;
                }
                if(currentActiveDialoguePoint.sentences == null || currentActiveDialoguePoint.sentences.Count ==0)
                {
                    Debug.LogWarning("该对话点没有台词");
                    return;
                }
                if(MiniGameDialoguePanel.Instance == null)
                {
                    Debug.LogError("MiniGameDialoguePanel实例不存在");
                    return;
                }
                isInteracting = true;
                MiniGameDialoguePanel.Instance.OpenDialogue(currentActiveDialoguePoint, OnDialogueEndCallback);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("T键代码异常：" + ex.Message);
            }
        }
    }

    // 对话结束回调，由面板调用
    void OnDialogueEndCallback(DialoguePoint finishedPoint)
    {
        finishedPoint.isCompleted = true;
        isInteracting = false;
        currentActiveDialoguePoint = null;
        CheckAllDialoguesDone();
    }

    // 检查所有对话是否全部完成
    public bool CheckAllDialoguesDone()
    {
        bool allDone = true;
        foreach(var dp in dialoguePoints)
        {
            if(!dp.isCompleted)
            {
                allDone = false;
                break;
            }
        }
        if(allDone) Debug.Log("✅全部对话完成，可以前往终点！");
        return allDone;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果碰到的是【终点触发器】
        if(other.gameObject.name == endTriggerName)
        {
            // 全部对话完成才跳转
            if(CheckAllDialoguesDone())
            {
                Debug.Log("✅全部对话完成，执行EndGame跳转");
                EndGame();
            }
            else
            {
                Debug.Log("❌还有对话没有完成，不能通关");
            }
            return;
        }

        // 下面是普通对话触发器逻辑
        Debug.Log($"【进入碰撞】碰到物体名称：{other.gameObject.name}");
        if(isInteracting) return;

        var findPoint = dialoguePoints.Find(p => p.pointName == other.gameObject.name);
        // 只有找到对话点 并且对话未完成，才启用提示文字
        if(findPoint != null && !findPoint.isCompleted)
        {
            currentActiveDialoguePoint = findPoint;
            if(tText != null) tText.SetActive(true);
            Debug.Log($"✅ 匹配成功！当前对话点：{findPoint.pointName}");
        }
        else
        {
            // 找不到对话点 OR 对话已经完成，不显示提示
            if(tText != null) tText.SetActive(false);
            Debug.Log($"❌ 找不到对应的对话点，或对话已完成。触发器名字：{other.gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"【离开碰撞】离开物体名称：{other.gameObject.name}");

        if(tText != null) tText.SetActive(false);
        var findPoint = dialoguePoints.Find(p => p.pointName == other.gameObject.name);
        if(findPoint == currentActiveDialoguePoint)
        {
            currentActiveDialoguePoint = null;
            Debug.Log("🔸 已清空当前对话点");
        }
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
            VNManager.Instance.BackTextGame("11");
        }
    }
}
