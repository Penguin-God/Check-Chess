using System.Collections.Generic;
using System.Linq;

public static class StageLogic
{
    public static HashSet<StageCoord> GetDesignatedLockLevels(IEnumerable<string> coords) => new HashSet<StageCoord>(coords.Select(c => StageCoord.FromBoardCoord(BoardCoord.FromChessSquare(c))));
    public static string SerializeUnlocked(HashSet<StageCoord> unlockedSet) => string.Join(",", unlockedSet.Select(coord => coord.ToAbsoluteLevel()));

    // 불러올 때는 int를 파싱한 뒤, FromAbsoluteLevel을 통해 StageCoord 레코드로 복원합니다.
    public static HashSet<StageCoord> DeserializeUnlocked(string data)
    {
        var set = new HashSet<StageCoord>();
        if (string.IsNullOrEmpty(data)) return set;

        foreach (var s in data.Split(','))
            if (int.TryParse(s, out int val)) set.Add(StageCoord.FromAbsoluteLevel(val));

        return set;
    }

    // StageCoord 객체 자체를 받아 해시셋(HashSet)에 포함되어 있는지 검사합니다!
    public static bool IsCurrentlyLocked(StageCoord level, HashSet<StageCoord> lockedSet, HashSet<StageCoord> unlockedSet) => lockedSet.Contains(level) && !unlockedSet.Contains(level);
}