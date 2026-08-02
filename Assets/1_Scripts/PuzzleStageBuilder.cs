
public static class PuzzleStageBuilder
{
    public static GameState CreateFromSO(StageDataSO stageData)
    {
        var board = new Board<PieceType>(coord => PieceType.None);

        foreach (var setup in stageData.initialPieces)
        {
            var coord = new BoardCoord(setup.X, setup.Y);
            board = board.Change(coord, setup.Piece);
        }

        return new GameState(board, null);
    }
}