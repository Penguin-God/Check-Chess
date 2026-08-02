using System;
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

public record Board<T>
{
    public const int Size = 8;
    private readonly T[,] _grid;
    public T[,] Grid => (T[,])_grid.Clone();

    public Board() => _grid = new T[Size, Size];

    public Board(T[,] grid)
    {
        if (grid.GetLength(0) != Size || grid.GetLength(1) != Size)
            throw new ArgumentException($"보드 크기는 반드시 {Size}x{Size}여야 합니다.");

        _grid = grid;
    }

    // 사용 예: var piece = board[new BoardCoord(3, 4)];
    public T this[BoardCoord coord]
    {
        get => _grid[coord.X, coord.Y];
    }

    // 값을 변경할 때 기존 보드를 수정하지 않고 "새로운 보드"를 반환
    // 사용 예: var newBoard = board.Set(new BoardCoord(3, 4), PieceType.Knight);
    public Board<T> Change(BoardCoord coord, T value)
    {
        // 얕은 복사 (값 타입이나 enum인 PieceType에 완벽하게 동작)
        var newGrid = (T[,])_grid.Clone();
        newGrid[coord.X, coord.Y] = value;

        return new Board<T>(newGrid);
    }
    public void ForEach(Action<BoardCoord, T> action)
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                var coord = new BoardCoord(x, y);
                action(coord, _grid[x, y]);
            }
        }
    }
}