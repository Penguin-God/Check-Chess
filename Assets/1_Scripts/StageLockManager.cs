using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    public List<string> lockedStageCoords;

    public HashSet<StageCoord> DesignatedLockLevels { get; private set; }
    public HashSet<StageCoord> UnlockedLevels { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        DesignatedLockLevels = StageLogic.GetDesignatedLockLevels(lockedStageCoords);
        LoadUnlockedLevels();
    }

    public void UnlockLevel(StageCoord stage)
    {
        UnlockedLevels.Add(stage);
        SaveUnlockedLevels();
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