

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

public record ChessSquare(int X, int Y);

public record PuzzlePlayState(PuzzleStatusType Status, PiecesType Piece);

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
        bool _isOutOfBounds = square.X < 0 || square.X >= Width || square.Y < 0 || square.Y >= Height;
        if (_isOutOfBounds) return null;
        return grid[square.X, square.Y];
    }
}