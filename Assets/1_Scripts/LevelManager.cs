using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("레벨 데이터베이스 SO")]
    public LevelDatabaseSO levelDatabase; // 리스트 대신 SO를 직접 참조합니다.
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
        if (levelDatabase == null) return null;

        bool isValidIndex = CurrentStage.ChapterIndex < levelDatabase.chapters.Count &&
                            CurrentStage.StageIndex < levelDatabase.chapters[CurrentStage.ChapterIndex].stages.Count;

        return isValidIndex ? levelDatabase.chapters[CurrentStage.ChapterIndex].stages[CurrentStage.StageIndex] : null;
    }

    public void ClearCurrentStage()
    {
        var nextStage = CurrentStage;
        nextStage++;

        if (nextStage > LocalStorage.LoadMaxPlayableStage())
            LocalStorage.SaveMaxPlayableStage(nextStage);
    }

    public bool CurrentStagePlayable(StageCoord stage) => stage <= LocalStorage.LoadMaxPlayableStage() && stage <= LocalStorage.LoadMaxClearableStage();
}