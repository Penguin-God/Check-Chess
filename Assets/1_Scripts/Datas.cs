using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public record BoardCoord(int X, int Y);

public record GameState(Board<PieceType> Board, BoardCoord ActiveSquare, IReadOnlyList<BoardCoord> AllowedStartingSquares)
{
    public bool IsKingCaptured => ActiveSquare != null && Board[ActiveSquare] == PieceType.King;
    public int RemainingPiecesCount => Board.GetAll().Count(piece => piece != PieceType.None);

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

    static IEnumerable<BoardCoord> AllCoords
    {
        get
        {
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    yield return new BoardCoord(x, y);
        }
    }

    static U[,] CreateGrid<U>(Func<BoardCoord, U> generator)
    {
        var grid = new U[Size, Size];
        foreach (var coord in AllCoords)
            grid[coord.X, coord.Y] = generator(coord);
        return grid;
    }

    public Board() => _grid = new T[Size, Size];

    public Board(T[,] grid)
    {
        if (grid.GetLength(0) != Size || grid.GetLength(1) != Size)
            throw new ArgumentException($"보드 크기는 반드시 {Size}x{Size}여야 합니다.");
        _grid = grid;
    }

    public Board(Func<BoardCoord, T> generator) => _grid = CreateGrid(generator);

    public T this[BoardCoord coord] => _grid[coord.X, coord.Y];

    public Board<T> Change(BoardCoord coord, T value)
    {
        var newGrid = (T[,])_grid.Clone();
        newGrid[coord.X, coord.Y] = value;
        return new Board<T>(newGrid);
    }

    public IEnumerable<T> GetAll() => AllCoords.Select(coord => this[coord]);
    public void ForEach(Action<BoardCoord, T> action) => AllCoords.ToList().ForEach(coord => action(coord, this[coord]));
    public Board<TResult> Map<TResult>(Func<BoardCoord, T, TResult> selector) => new Board<TResult>(CreateGrid(coord => selector(coord, this[coord])));
    public IEnumerable<KeyValuePair<BoardCoord, T>> GetAllSquares() => AllCoords.Select(coord => new KeyValuePair<BoardCoord, T>(coord, this[coord]));
    public Dictionary<BoardCoord, T> ToDictionary() => AllCoords.ToDictionary(coord => coord, coord => this[coord]);
}