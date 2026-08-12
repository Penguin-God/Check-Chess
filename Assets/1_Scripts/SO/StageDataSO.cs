using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PieceSetup
{
    public int X;
    public int Y;
    public PieceType Piece;
}

[CreateAssetMenu(fileName = "NewStage", menuName = "ChessPuzzle/StageData")]
public class StageDataSO : ScriptableObject
{
    [Header("초기 기물 배치")]
    public List<PieceSetup> initialPieces = new List<PieceSetup>();

    [Header("힌트 좌표")]
    public Vector2Int startHintCoord = new Vector2Int(-1, -1);
    public Vector2Int nextHintCoord = new Vector2Int(-1, -1);

    // 첫 번째 힌트(시작 기물) 좌표 반환
    public BoardCoord GetStartHintCoord()
    {
        if (startHintCoord.x < 0 || startHintCoord.y < 0) return null;
        return new BoardCoord(startHintCoord.x, startHintCoord.y);
    }

    // 두 번째 힌트(다음 기물) 좌표 반환
    public BoardCoord GetNextHintCoord()
    {
        if (nextHintCoord.x < 0 || nextHintCoord.y < 0) return null;
        return new BoardCoord(nextHintCoord.x, nextHintCoord.y);
    }
}