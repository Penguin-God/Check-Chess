using System.Collections.Generic;
using UnityEngine;

public enum StageState
{
    Locked,
    Playable,
    Unreached
}

public static class StageStatusLogic
{
    public static StageState EvaluateStageState(StageCoord currentStage, StageCoord maxClearedStage, HashSet<StageCoord> lockedSet, HashSet<StageCoord> unlockedSet)
    {
        if (StageLogic.IsCurrentlyLocked(currentStage, lockedSet, unlockedSet)) return StageState.Locked;
        if (currentStage <= maxClearedStage) return StageState.Playable;

        return StageState.Unreached;
    }

    public static StageState EvaluateStageState(StageCoord currentStage, StageCoord maxClearedStage, StageCoord maxUnLockStage)
    {
        if (maxClearedStage == maxUnLockStage) return StageState.Locked;
        if (currentStage <= maxClearedStage) return StageState.Playable;
        return StageState.Unreached;
    }

    public static Color GetStatusColor(StageState state, Color baseColor) => state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
}
