using System.Collections.Generic;
using UnityEngine;

public class StageLockManager : MonoBehaviour
{
    public HashSet<StageCoord> CurrentLockPoints => StageLogic.GetRemainingLocks(lockPointDataSO.GetLockPoints(), LocalStorage.LoadMaxClearableStage());
    [SerializeField] LockPointDataSO lockPointDataSO;

    void Awake()
    {
        // 최초 unlock
        if (LocalStorage.LoadMaxClearableStage() == new StageCoord(0, 0))
            SaveMaxClearableStage(new StageCoord(0, 0));
    }

    public void SaveMaxClearableStage(StageCoord unlockPoint)
    {
        var maxClearalbe = StageLogic.GetClearableLimit(lockPointDataSO.GetLockPoints(), unlockPoint);
        LocalStorage.SaveMaxClearableStage(maxClearalbe);
    }
}