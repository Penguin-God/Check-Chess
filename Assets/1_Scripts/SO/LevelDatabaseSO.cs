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

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "ChessPuzzle/LevelDatabase")]
public class LevelDatabaseSO : ScriptableObject
{
    [Header("전체 챕터 및 스테이지 데이터")]
    public List<ChapterData> chapters;

#if UNITY_EDITOR
    [ContextMenu("스테이지 데이터 자동 세팅 및 텍스트 채우기 (a1~h8)")]
    void SetupStagesAutomatically()
    {
        chapters = new List<ChapterData>();
        string basePath = "Assets/GameResources/Stages";

        for (char c = 'a'; c <= 'h'; c++)
        {
            ChapterData newChapter = new ChapterData
            {
                chapterName = $"Chapter {char.ToUpper(c)}",
                stages = new List<StageDataSO>()
            };

            for (int i = 1; i <= 8; i++)
            {
                string stageName = $"{c}{i}";
                string fullPath = $"{basePath}/{stageName}.asset";

                StageDataSO stageData = AssetDatabase.LoadAssetAtPath<StageDataSO>(fullPath);

                if (stageData == null)
                {
                    Debug.LogWarning($"[LevelDatabase] '{stageName}' 스테이지 파일이 없어서 null로 비워둡니다. 경로: {fullPath}");
                }
                else
                {
                    // 텍스트가 비어있으면 파일명(보드 위치)을 기본값으로 채워줍니다.
                    if (string.IsNullOrEmpty(stageData.StageText))
                    {
                        stageData.StageText = stageName;
                        EditorUtility.SetDirty(stageData); // 해당 SO 파일이 수정되었음을 유니티에 알림
                    }
                }

                newChapter.stages.Add(stageData);
            }

            chapters.Add(newChapter);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets(); // SO 파일들의 텍스트 변경 사항을 디스크에 확실히 저장
        Debug.Log("[LevelDatabase] 총 64개의 스테이지 데이터 매핑 및 기본 텍스트 세팅이 완료되었습니다!");
    }
#endif
}