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

    public StageCoord CurrentStage { get; set; }

    int MaxClearedLevel
    {
        get => LocalStorage.LoadMaxClearedLevel();
        set => LocalStorage.SaveMaxClearedLevel(value);
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

    public StageDataSO GetCurrentStageData()
    {
        bool isValidIndex = CurrentStage.ChapterIndex >= 0 && CurrentStage.ChapterIndex < chapters.Count &&
                            CurrentStage.StageIndex >= 0 && CurrentStage.StageIndex < chapters[CurrentStage.ChapterIndex].stages.Count;

        return isValidIndex ? chapters[CurrentStage.ChapterIndex].stages[CurrentStage.StageIndex] : null;
    }

    public void ClearCurrentStage()
    {
        if (CurrentStage.ToAbsoluteLevel() >= MaxClearedLevel)
            MaxClearedLevel = CurrentStage.ToAbsoluteLevel() + 1;
    }
}