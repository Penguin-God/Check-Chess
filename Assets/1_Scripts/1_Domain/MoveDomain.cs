public static class PuzzleDomain
{
    //public static Option<ChessSquare> ValidateSquare(ChessSquare targetSquare, ImmutableList<ChessSquare> allowedSquares)
    //{
    //    bool _isAllowed = allowedSquares.Contains(targetSquare);

    //    return _isAllowed
    //        ? Option<ChessSquare>.Some(targetSquare)
    //        : Option<ChessSquare>.None;
    //}

    //public static Option<PiecesType> GetPiece(BoardState boardState, ChessSquare targetSquare)
    //{
    //    PiecesType? _foundPiece = boardState.GetPieceAt(targetSquare);

    //    return _foundPiece.HasValue
    //        ? Option<PiecesType>.Some(_foundPiece.Value)
    //        : Option<PiecesType>.None;
    //}

    //public static PuzzlePlayState CreatePuzzlePlayState(PiecesType selectedPiece)
    //{
    //    PuzzlePlayState _newState = new PuzzlePlayState(PuzzleStatusType.Select, selectedPiece);
    //    return _newState;
    //}
}