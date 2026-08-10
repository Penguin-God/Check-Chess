using System.Collections.Generic;
using System.Linq;

public static class StageLogic
{
    public static StageCoord GetPlayableLimit(HashSet<StageCoord> lockedSet, StageCoord maxUnlockStage)
    {
        // unlockStage보다 큰 자물쇠들만 필터링한 뒤, 오름차순 정렬하여 가장 가까운 첫 번째 자물쇠를 찾습니다.
        StageCoord nextLock =
            GetRemainingLocks(lockedSet, maxUnlockStage)
            .OrderBy(lockStage => lockStage)
            .FirstOrDefault();

        // [예외 처리] 이제 앞을 막고 있는 자물쇠가 없다면? (다 풀었거나 애초에 자물쇠가 없을 때)
        if (nextLock == null) return StageCoord.MaxStage;

        // 첫 번째 자물쇠의 바로 이전 단계(--)를 반환합니다.
        return --nextLock;
    }

    // unlockStage보다 큰 스테이지만 가져옴
    public static HashSet<StageCoord> GetRemainingLocks(HashSet<StageCoord> lockedSet, StageCoord maxUnlockStage) => new (lockedSet.Where(lockStage => lockStage > maxUnlockStage));

    public static HashSet<StageCoord> GetDesignatedLockLevels(IEnumerable<string> coords) => new (coords.Select(StringToStage));
    static StageCoord StringToStage(string coord) => StageCoord.FromBoardCoord(BoardCoord.FromChessSquare(coord));

    public static string SerializeUnlocked(HashSet<StageCoord> unlockedSet) => string.Join(",", unlockedSet.Select(coord => coord.ToAbsoluteLevel()));

    // 불러올 때는 int를 파싱한 뒤, FromAbsoluteLevel을 통해 StageCoord 레코드로 복원합니다.
    public static HashSet<StageCoord> DeserializeUnlocked(string data)
    {
        var set = new HashSet<StageCoord>();
        if (string.IsNullOrEmpty(data)) return set;

        foreach (var s in data.Split(','))
        {
            if (int.TryParse(s, out int val))
                set.Add(StageCoord.FromAbsoluteLevel(val));
        }

        return set;
    }

    // StageCoord 객체 자체를 받아 해시셋(HashSet)에 포함되어 있는지 검사합니다!
    public static bool IsCurrentlyLocked(StageCoord level, HashSet<StageCoord> lockedSet, HashSet<StageCoord> unlockedSet) => lockedSet.Contains(level) && !unlockedSet.Contains(level);
}