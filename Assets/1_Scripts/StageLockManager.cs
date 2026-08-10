using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public static StageLockManager Instance { get; private set; }

    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    [SerializeField] List<string> lockPointCoords;

    public HashSet<StageCoord> CurrentLockPoints => StageLogic.GetRemainingLocks(StageLogic.GetDesignatedLockLevels(lockPointCoords), LocalStorage.LoadMaxUnlockStage());
    
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    public void UnlockLevel(StageCoord stage) => LocalStorage.SaveMaxUnlockStage(stage);
    public StageState EvaluateStageState(StageCoord stage) => StageStatusLogic.EvaluateStageState(stage, LocalStorage.LoadMaxClearedStage(), LocalStorage.LoadMaxUnlockStage());
}