

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

public class BoardState
{
    
}