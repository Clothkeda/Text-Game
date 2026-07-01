using System.Collections;
using System.Collections.Generic;
using ExcelDataReader;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class VNManager : MonoBehaviour
{
    public Text speakerName;
    public Text speakingContent;

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
        var data = storyData[currentLine];
        speakerName.text = data.speaker;
        speakingContent.text = data.content;
        currentLine++;
    }
}
