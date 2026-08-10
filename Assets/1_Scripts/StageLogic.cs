using System.Collections.Generic;
using System.Linq;

public static class StageLogic
{
    public static StageCoord GetClearableLimit(HashSet<StageCoord> lockedSet, StageCoord maxUnlockStage)
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

    public static HashSet<StageCoord> StringToStage(IEnumerable<string> coords) => new (coords.Select(StringToStage));
    static StageCoord StringToStage(string coord) => StageCoord.FromBoardCoord(BoardCoord.FromChessSquare(coord));
}