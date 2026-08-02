using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public record BoardCoord(int X, int Y);

public record GameState(IReadOnlyDictionary<BoardCoord, PieceType> Board, BoardCoord ActiveSquare, IReadOnlyList<BoardCoord> AllowedStartingSquares)
{
    // 딕셔너리에서 ActiveSquare 좌표로 기물을 바로 가져와서 확인합니다.
    public bool IsKingCaptured =>
        ActiveSquare != null &&
        Board.TryGetValue(ActiveSquare, out var piece) &&
        piece == PieceType.King;

    public int RemainingPiecesCount => Board.Values.Count(piece => piece != PieceType.None);

    // 승리 조건: 킹을 잡았고, 필드에 남은 기물이 킹(1개)뿐일 때 완벽한 클리어입니다
    public bool IsVictory => IsKingCaptured && RemainingPiecesCount == 1;
    // 패배 조건: 킹을 잡았지만, 필드에 킹 이외의 다른 기물이 남아있을 때입니다
    public bool IsDefeat => IsKingCaptured && RemainingPiecesCount > 1;
}