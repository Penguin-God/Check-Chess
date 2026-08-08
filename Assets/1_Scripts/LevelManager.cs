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

    // 프로퍼티 내부에서 LocalStorage를 호출하여 결합도를 낮춤
    public int MaxClearedLevel
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
        var (chapterIdx, stageIdx) = StageLogic.ToChapterAndStageIndices(absoluteLevel, Board<int>.Size);

        // 2. 인덱스가 유효한 범위 내에 있는지 판별합니다.
        bool isValidIndex = chapterIdx >= 0 && chapterIdx < chapters.Count &&
                            stageIdx >= 0 && stageIdx < chapters[chapterIdx].stages.Count;

        // 3. 유효하면 데이터를, 아니면 null을 반환합니다. (함수형의 단일 반환 원칙)
        return isValidIndex ? chapters[chapterIdx].stages[stageIdx] : null;
    }

    public void ClearCurrentStage()
    {
        if (CurrentAbsoluteLevel >= MaxClearedLevel)
            MaxClearedLevel = CurrentAbsoluteLevel + 1;
    }
}