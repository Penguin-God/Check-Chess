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
    public static Color GetStatusColor(StageState state, Color baseColor) => state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
}
