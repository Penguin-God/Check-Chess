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

    void Awake()
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
        bool isValidIndex = CurrentStage.ChapterIndex < chapters.Count && CurrentStage.StageIndex < chapters[CurrentStage.ChapterIndex].stages.Count;
        return isValidIndex ? chapters[CurrentStage.ChapterIndex].stages[CurrentStage.StageIndex] : null;
    }

    public void ClearCurrentStage()
    {
        var nextStage = CurrentStage;
        nextStage++;

        if (nextStage > LocalStorage.LoadMaxPlayableStage())
            LocalStorage.SaveMaxPlayableStage(nextStage);
    }

    // 다음 스테이지로 넘어갈 수 있는지 확인 (진행도 조건 && 자물쇠 조건)
    public bool CurrentStagePlayable(StageCoord stage) => stage <= LocalStorage.LoadMaxPlayableStage() && stage <= LocalStorage.LoadMaxClearableStage();
}