using System.Collections.Generic;
using System.Linq;

public static class StageLockLogic
{
    // 좌표 문자열을 절대 레벨 숫자로 변환
    public static int ParseCoord(string coord, int boardSize)
    {
        if (string.IsNullOrEmpty(coord) || coord.Length < 2) return -1;
        int x = coord.ToLower()[0] - 'a';
        int y = int.Parse(coord[1].ToString()) - 1;
        return (x * boardSize) + y;
    }

    // 잠금 지정된 좌표 리스트를 레벨 집합으로 변환
    public static HashSet<int> GetDesignatedLockLevels(IEnumerable<string> coords, int boardSize) =>
        new HashSet<int>(coords.Select(c => ParseCoord(c, boardSize)));

    // 데이터 직렬화/역직렬화
    public static string SerializeUnlocked(HashSet<int> unlockedSet) => string.Join(",", unlockedSet);
    public static HashSet<int> DeserializeUnlocked(string data)
    {
        var set = new HashSet<int>();
        if (string.IsNullOrEmpty(data)) return set;

        foreach (var s in data.Split(','))
            if (int.TryParse(s, out int val)) set.Add(val);

        return set;
    }

    // --- 상태 판별 순수 함수들 ---

    // 자물쇠 칸인가?
    public static bool IsLock(int level, HashSet<int> lockedSet) => lockedSet.Contains(level);

    // 유저가 자물쇠를 풀었는가?
    public static bool IsUnlocked(int level, HashSet<int> unlockedSet) => unlockedSet.Contains(level);

    // 일반적인 진행도로 여기까지 도달했는가?
    public static bool IsReached(int level, int maxCleared) => level <= maxCleared;

    // [핵심] 현재 이 칸이 잠겨있는 상태인가? (잠금 칸으로 지정되었는데, 아직 안 풀었음)
    public static bool IsCurrentlyLocked(int level, HashSet<int> lockedSet, HashSet<int> unlockedSet) => IsLock(level, lockedSet) && !IsUnlocked(level, unlockedSet);
}