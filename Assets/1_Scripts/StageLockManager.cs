using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    public List<string> lockedStageCoords;

    public HashSet<StageCoord> LockLevels { get; private set; }
    public HashSet<StageCoord> UnlockedLevels { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        LockLevels = StageLogic.GetDesignatedLockLevels(lockedStageCoords);
        LoadUnlockedLevels();
    }

    public void UnlockLevel(StageCoord stage)
    {
        UnlockedLevels.Add(stage);
        SaveUnlockedLevels();
    }

    public void _UnlockLevel(StageCoord stage)
    {
        StageLogic.GetPlayableLimit(LockLevels, stage);
        LocalStorage.SaveMaxUnlockStage(stage);
    }

    void SaveUnlockedLevels()
    {
        string data = StageLogic.SerializeUnlocked(UnlockedLevels);
        LocalStorage.SaveUnlockedStages(data);
    }

    void LoadUnlockedLevels()
    {
        string data = LocalStorage.LoadUnlockedStages();
        UnlockedLevels = StageLogic.DeserializeUnlocked(data);
    }
}