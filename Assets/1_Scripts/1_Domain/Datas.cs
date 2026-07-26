public enum PiecesType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum PuzzleStatusType
{
    Select,
    Action
}

public record ChessSquare(int x, int y);

public record PuzzlePlayState(PuzzleStatusType status, PiecesType piece);

public record BoardState
{
    private readonly PiecesType?[,] grid;

    public int Width { get; }
    public int Height { get; }

    public BoardState(PiecesType?[,] initialGrid)
    {
        grid = initialGrid;
        Width = grid.GetLength(0);
        Height = grid.GetLength(1);
    }

    public PiecesType? GetPieceAt(ChessSquare square)
    {
        bool _isOutOfBounds = square.x < 0 || square.x >= Width || square.y < 0 || square.y >= Height;
        if (_isOutOfBounds) return null;
        return grid[square.x, square.y];
    }
}

public static class DataFactory
{
    public static ChessSquare CreateSquare(int x, int y) => new ChessSquare(x, y);
}