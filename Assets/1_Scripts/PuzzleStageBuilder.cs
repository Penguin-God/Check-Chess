using System.Collections.Generic;

public static class PuzzleStageBuilder
{
    public static GameState CreateFromSO(StageDataSO stageData)
    {
        // 1. IReadOnlyDictionary로 변환하기 위해 일반 Dictionary 생성
        var board = new Dictionary<BoardCoord, PieceType>();

        // 2. 8x8 보드를 순회하며 기본값(None)으로 64개의 칸을 모두 초기화
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                board[new BoardCoord(x, y)] = PieceType.None;
            }
        }

        var allowedStarts = new List<BoardCoord>();

        // 3. SO 데이터에 설정된 기물들을 보드에 배치
        foreach (var setup in stageData.initialPieces)
        {
            // 리스트의 인덱스(y * 8 + x) 계산 대신 직관적인 좌표 객체 사용
            var coord = new BoardCoord(setup.X, setup.Y);

            board[coord] = setup.Piece;

            if (setup.DisableStart == false)
            {
                allowedStarts.Add(coord);
            }
        }

        return new GameState(board, null, allowedStarts);
    }
}