using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChapterData
{
    public string chapterName;
    public List<StageDataSO> stages = new List<StageDataSO>(10);
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("전체 챕터 및 스테이지 데이터")]
    public List<ChapterData> chapters;

    // 현재 플레이 중인 절대 레벨 (0부터 시작)
    public int CurrentAbsoluteLevel { get; set; }

    public int MaxClearedLevel
    {
        get => PlayerPrefs.GetInt("MaxClearedLevel", 0);
        set => PlayerPrefs.SetInt("MaxClearedLevel", value);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public StageDataSO GetStageData(int absoluteLevel)
    {
        int chapterIdx = absoluteLevel / 8;
        int stageIdx = absoluteLevel % 8;

        if (chapterIdx >= 0 && chapterIdx < chapters.Count)
        {
            if (stageIdx >= 0 && stageIdx < chapters[chapterIdx].stages.Count)
            {
                return chapters[chapterIdx].stages[stageIdx];
            }
        }
        return null;
    }

    public void ClearCurrentStage()
    {
        if (CurrentAbsoluteLevel >= MaxClearedLevel)
            MaxClearedLevel = CurrentAbsoluteLevel + 1;
    }
}