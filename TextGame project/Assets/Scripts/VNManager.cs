using System.Collections;
using System.Collections.Generic;
using ExcelDataReader;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class VNManager : MonoBehaviour
{
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakingContent;
    public TypewriterEffect typewriterEffect;

    private string filePath = Constants.STORY_PATH;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = 0;
    
    void Start()
    {
        LoadStoryFromFile(filePath);
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
            Debug.LogError("No data found in the file");
        }
    }

    void DisplayNextLine()
    {
        if (currentLine >= storyData.Count)
        {
            Debug.Log("End of story");
            return;
        }

        if (typewriterEffect.IsTyping())
        {
            typewriterEffect.CompleteLine();
        }
        else
        {
            var data = storyData[currentLine];
            speakerName.text = data.speaker;
            typewriterEffect.StartTyping(data.content);
            speakingContent.text = data.content;
            currentLine++;
        }
        
    }
}
