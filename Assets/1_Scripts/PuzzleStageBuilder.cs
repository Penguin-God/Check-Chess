using System.Collections.Generic;

public static class PuzzleStageBuilder
{
    public static GameState CreateFromSO(StageDataSO stageData)
    {
        var board = new Board<PieceType>(coord => PieceType.None);
        var allowedStarts = new List<BoardCoord>();

        // 3. SO 데이터에 설정된 기물들을 보드에 배치
        foreach (var setup in stageData.initialPieces)
        {
            var coord = new BoardCoord(setup.X, setup.Y);
            board = board.Change(coord, setup.Piece);

            if (setup.DisableStart == false)
            {
                allowedStarts.Add(coord);
            }
        }

        return new GameState(board, null, allowedStarts);
    }
}