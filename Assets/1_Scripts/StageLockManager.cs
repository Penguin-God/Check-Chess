using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    [SerializeField] List<string> lockPointCoords;

    HashSet<StageCoord> LockLevels { get; set; }
    public HashSet<StageCoord> LockPoints => StageLogic.GetDesignatedLockLevels(lockPointCoords);
    
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        LockLevels = StageLogic.GetDesignatedLockLevels(lockPointCoords);
    }

    public void UnlockLevel(StageCoord stage)
    {
        StageLogic.GetPlayableLimit(LockLevels, stage);
        LocalStorage.SaveMaxUnlockStage(stage);
    }

    public StageState EvaluateStageState(StageCoord stage) => StageStatusLogic.EvaluateStageState(stage, LocalStorage.LoadMaxClearedStage(), LocalStorage.LoadMaxUnlockStage());
}