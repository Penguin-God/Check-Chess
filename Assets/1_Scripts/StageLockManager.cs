using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    public List<string> lockedStageCoords;

    public HashSet<int> DesignatedLockLevels { get; private set; }
    public HashSet<int> UnlockedLevels { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        DesignatedLockLevels = StageLockLogic.GetDesignatedLockLevels(lockedStageCoords, 8);
        LoadUnlockedLevels();
    }

    public void UnlockLevel(int absoluteLevel)
    {
        UnlockedLevels.Add(absoluteLevel);
        SaveUnlockedLevels();
    }

    void SaveUnlockedLevels()
    {
        string data = StageLockLogic.SerializeUnlocked(UnlockedLevels);
        LocalStorage.SaveUnlockedStages(data);
    }

    void LoadUnlockedLevels()
    {
        string data = LocalStorage.LoadUnlockedStages();
        UnlockedLevels = StageLockLogic.DeserializeUnlocked(data);
    }
}