using System.Collections.Generic;
using UnityEngine;

public enum StageState
{
    Locked,
    Playable,
    Unreached
}

public static class LobbySquarePresenter
{
    public static StageState EvaluateStageState(StageCoord currentStage, StageCoord maxClearedStage, HashSet<int> lockedSet, HashSet<int> unlockedSet)
    {
        if (StageLogic.IsCurrentlyLocked(currentStage.ToAbsoluteLevel(), lockedSet, unlockedSet)) return StageState.Locked;
        if (currentStage <= maxClearedStage) return StageState.Playable;

        return StageState.Unreached;
    }

    public static Color GetSquareColor(BoardCoord coord, StageState state, Color baseColor) => state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
}
