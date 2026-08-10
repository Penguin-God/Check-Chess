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
    public static StageState EvaluateStageState(StageCoord stage, StageCoord maxClearedStage, StageCoord maxUnLockStage)
    {
        if (stage > maxUnLockStage) return StageState.Locked;
        if (stage <= maxClearedStage) return StageState.Playable;
        return StageState.Unreached;
    }

    public static Color GetStatusColor(StageState state, Color baseColor) => state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
    public static Color GetStatusColor(StagePersentType state, Color baseColor) => state == StagePersentType.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
}
