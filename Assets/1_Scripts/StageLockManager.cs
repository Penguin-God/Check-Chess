using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    public List<string> lockedStageCoords = new List<string> { "b1", "c3", "d6" };

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
        PlayerPrefs.SetString("UnlockedStageLevels", data); // 저장 키 이름도 직관적으로 변경
    }

    void LoadUnlockedLevels()
    {
        string data = PlayerPrefs.GetString("UnlockedStageLevels", "");
        UnlockedLevels = StageLockLogic.DeserializeUnlocked(data);
    }
}