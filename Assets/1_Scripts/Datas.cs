using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }

public record BoardCoord(int X, int Y)
{
    public string ToChessSquare() => $"{(char)('a' + X)}{Y + 1}";

    public static BoardCoord FromChessSquare(string square)
    {
        if (square.Length < 2) throw new ArgumentException($"유효하지 않은 체스 좌표입니다: {square}");

        // 아스키 코드로 치환하면 숫자라서 간단 연산으로 계산 가능
        return new BoardCoord(square.ToLower()[0] - 'a', square[1] - '1');
    }
}

public record GameState(Board<PieceType> Board, BoardCoord ActiveSquare)
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
    public static readonly int Size = 8;
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

public static class BoardSize
{
    public const int Size = 8; // 절대 불변 고정값
}

public record StageCoord(int ChapterIndex, int StageIndex) : IComparable<StageCoord>
{
    public static StageCoord FromAbsoluteLevel(int absoluteLevel) => new StageCoord(absoluteLevel / BoardSize.Size, absoluteLevel % BoardSize.Size);
    public static StageCoord FromBoardCoord(BoardCoord boardCoord) => new StageCoord(boardCoord.X, boardCoord.Y);
    public int ToAbsoluteLevel() => (ChapterIndex * BoardSize.Size) + StageIndex;
    public BoardCoord ToBoardCoord() => new BoardCoord(ChapterIndex, StageIndex);

    // 1. IComparable 구현 (리스트의 Sort() 등 정렬을 지원하기 위한 표준)
    public int CompareTo(StageCoord other)
    {
        if (other == null) return 1; // 절대 레벨 값으로 비교를 위임합니다.
        return ToAbsoluteLevel().CompareTo(other.ToAbsoluteLevel());
    }

    // 2. 비교 연산자 오버로딩 (ToAbsoluteLevel을 활용한 아주 간결한 구현)
    public static bool operator >(StageCoord left, StageCoord right) => left.ToAbsoluteLevel() > right.ToAbsoluteLevel();
    public static bool operator >=(StageCoord left, StageCoord right) => left.ToAbsoluteLevel() >= right.ToAbsoluteLevel();

    public static bool operator <(StageCoord left, StageCoord right) => left.ToAbsoluteLevel() < right.ToAbsoluteLevel();
    public static bool operator <=(StageCoord left, StageCoord right) => left.ToAbsoluteLevel() <= right.ToAbsoluteLevel();

    public static StageCoord operator ++(StageCoord coord) => FromAbsoluteLevel(coord.ToAbsoluteLevel() + 1);

    public static StageCoord operator --(StageCoord coord)
    {
        // 0 레벨 미만으로 떨어지는 것을 방지(Math.Max)하면서 1을 뺍니다.
        int prevLevel = System.Math.Max(0, coord.ToAbsoluteLevel() - 1);
        return FromAbsoluteLevel(prevLevel);
    }
}