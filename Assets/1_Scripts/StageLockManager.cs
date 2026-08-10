using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    [SerializeField] List<string> lockPointCoords;

    public HashSet<StageCoord> CurrentLockPoints => StageLogic.GetRemainingLocks(StageLogic.GetDesignatedLockLevels(lockPointCoords), LocalStorage.LoadMaxClearableStage());
    
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        // 최초 unlock
        if (LocalStorage.LoadMaxClearableStage() == new StageCoord(0, 0))
            SaveMaxClearableStage(new StageCoord(0, 0));
    }

    public void SaveMaxClearableStage(StageCoord unlockPoint)
    {
        var maxClearalbe = StageLogic.GetClearableLimit(StageLogic.GetDesignatedLockLevels(lockPointCoords), unlockPoint);
        LocalStorage.SaveMaxClearableStage(maxClearalbe);
    }
}