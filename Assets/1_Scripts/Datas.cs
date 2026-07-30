using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public record ChessSquare(int X, int Y, PieceType Piece);
public record BoardCoord(int X, int Y);

public record GameState(IReadOnlyList<ChessSquare> Board, ChessSquare ActiveSquare, IReadOnlyList<ChessSquare> AllowedStartingSquares)
{
    // 킹을 잡았는지 여부를 판별합니다.
    public bool IsKingCaptured => ActiveSquare != null && ActiveSquare.Piece == PieceType.King;

    // 현재 보드에 존재하는(None이 아닌) 기물의 총 개수를 구합니다.
    // 킹을 잡은 시점이라면, 방금 잡은 킹 자신이 ActiveSquare가 되어 보드에 1개로 집계됩니다[cite: 1].
    public int RemainingPiecesCount => Board.Count(sq => sq.Piece != PieceType.None);

    // 승리 조건: 킹을 잡았고, 필드에 남은 기물이 킹(1개)뿐일 때 완벽한 클리어입니다[cite: 1].
    public bool IsVictory => IsKingCaptured && RemainingPiecesCount == 1;

    // 패배 조건: 킹을 잡았지만, 필드에 킹 이외의 다른 기물이 남아있을 때입니다[cite: 1].
    public bool IsDefeat => IsKingCaptured && RemainingPiecesCount > 1;
}