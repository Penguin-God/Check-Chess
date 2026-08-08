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

    public int CurrentAbsoluteLevel { get; set; }

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

    public StageDataSO GetStageData(int absoluteLevel)
    {
        StageCoord stageCoord = StageCoord.FromAbsoluteLevel(absoluteLevel);

        // 2. 레코드의 프로퍼티를 사용하여 유효성 검사 (가독성 대폭 향상)
        bool isValidIndex = stageCoord.ChapterIndex >= 0 && stageCoord.ChapterIndex < chapters.Count &&
                            stageCoord.StageIndex >= 0 && stageCoord.StageIndex < chapters[stageCoord.ChapterIndex].stages.Count;

        // 3. 유효하면 데이터를, 아니면 null을 반환
        return isValidIndex ? chapters[stageCoord.ChapterIndex].stages[stageCoord.StageIndex] : null;
    }

    public void ClearCurrentStage()
    {
        if (CurrentAbsoluteLevel >= MaxClearedLevel)
            MaxClearedLevel = CurrentAbsoluteLevel + 1;
    }
}