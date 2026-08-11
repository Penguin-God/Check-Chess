using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ChapterData
{
    public string chapterName;
    public List<StageDataSO> stages = new List<StageDataSO>(8);
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

    public bool CurrentStagePlayable(StageCoord stage) => stage <= LocalStorage.LoadMaxPlayableStage() && stage <= LocalStorage.LoadMaxClearableStage();

#if UNITY_EDITOR
    [ContextMenu("스테이지 데이터 자동 세팅 (a1~h8)")]
    void SetupStagesAutomatically()
    {
        chapters = new List<ChapterData>();
        string basePath = "Assets/GameResources/Stages";

        // 체스판의 가로축 (a ~ h)을 챕터로 간주합니다.
        for (char c = 'a'; c <= 'h'; c++)
        {
            ChapterData newChapter = new ChapterData
            {
                chapterName = $"Chapter {char.ToUpper(c)}", // 예: Chapter A
                stages = new List<StageDataSO>()
            };

            // 체스판의 세로축 (1 ~ 8)을 스테이지로 간주합니다.
            for (int i = 1; i <= 8; i++)
            {
                string stageName = $"{c}{i}";
                // ScriptableObject의 기본 확장자인 .asset을 붙여 경로를 완성합니다.
                string fullPath = $"{basePath}/{stageName}.asset";

                StageDataSO stageData = AssetDatabase.LoadAssetAtPath<StageDataSO>(fullPath);

                if (stageData == null)
                {
                    Debug.LogWarning($"[LevelManager] '{stageName}' 스테이지 파일이 없어서 null로 비워둡니다. 경로: {fullPath}");
                }

                newChapter.stages.Add(stageData);
            }

            chapters.Add(newChapter);
        }

        // 인스펙터의 변경 사항이 씬이나 프리팹에 확실히 저장되도록 Dirty 마킹을 해줍니다.
        EditorUtility.SetDirty(this);
        Debug.Log("[LevelManager] 총 64개의 스테이지 데이터 자동 매핑이 완료되었습니다!");
    }
#endif
}