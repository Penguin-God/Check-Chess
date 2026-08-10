using System.Collections.Generic;
using UnityEngine;

public enum StageState
{
    LockPoint,
    Playable,
    Unplayable,
}

public static class StageStatusLogic
{
    public static StageState EvaluateStageState(StageCoord stage, StageCoord maxClearedStage, StageCoord maxUnLockStage)
    {
        if (stage > maxUnLockStage) return StageState.LockPoint;
        if (stage <= maxClearedStage) return StageState.Playable;
        return StageState.Unplayable;
    }

    public static Color GetStatusColor(StageState state, Color baseColor) => state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
}
