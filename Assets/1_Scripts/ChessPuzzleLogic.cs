using System;
using System.Collections.Generic;
using System.Linq;

public static class ChessPuzzleLogic
{
    public static GameState SelectStartingPiece(GameState state, BoardCoord startSquare)
    {
        if (state.ActiveSquare != null) return state;

        // 빈 공간이거나 킹(King)이면 선택 불가
        var clickedPiece = state.Board[startSquare];
        if (clickedPiece == PieceType.None || clickedPiece == PieceType.King)
            return state;

        return state with { ActiveSquare = startSquare };
    }

    // [2] 바톤 터치가 가능한 칸 목록 반환
    public static IReadOnlyList<BoardCoord> GetValidBatonTouches(GameState state)
    {
        if (state.ActiveSquare == null) return Array.Empty<BoardCoord>();

        var active = state.ActiveSquare;

        // 1. 현재 보드에 존재하는 모든 기물을 가져오고 총개수를 셉니다.
        var allPieces = state.Board.GetAllSquares().Where(kvp => kvp.Value != PieceType.None).ToList();
        int totalPieceCount = allPieces.Count;

        // 2. 성능 최적화: IsValidChessMove에서 매번 딕셔너리로 변환하지 않도록 캐싱
        var boardDict = state.Board.ToDictionary();

        return allPieces
            .Where(kvp => kvp.Key != active && IsTargetCapturable(kvp.Value)) // 자기 자신(active) 제외
            .Select(kvp => kvp.Key)
            .Where(coord => IsValidChessMove(boardDict, active, coord))
            .ToList();

        // [내부 함수] 킹은 기물 수가 2개(자기 제외 하나 더)여야만 잡을 수 있음
        bool IsTargetCapturable(PieceType targetPiece)
        {
            if (targetPiece == PieceType.King) return totalPieceCount == 2;
            else return true;
        }
    }

    // [3] 바톤 터치 실행
    public static GameState MoveAndTouch(GameState state, BoardCoord targetSquare)
    {
        var validMoves = GetValidBatonTouches(state);

        // validMoves에서 이미 킹 접근을 차단했으므로, 조건이 안 맞으면 여기서 튕겨냅니다!
        if (!validMoves.Contains(targetSquare)) return state;

        var newBoard = state.Board.Change(state.ActiveSquare, PieceType.None);

        return state with
        {
            Board = newBoard,
            ActiveSquare = targetSquare
        };
    }

    // [4] 체스 이동 규칙 검증
    private static bool IsValidChessMove(IReadOnlyDictionary<BoardCoord, PieceType> board, BoardCoord from, BoardCoord to)
    {
        int dx = Math.Abs(from.X - to.X);
        int dy = Math.Abs(from.Y - to.Y);

        if (!board.TryGetValue(from, out var piece)) return false;

        return piece switch
        {
            PieceType.Knight => (dx == 1 && dy == 2) || (dx == 2 && dy == 1),
            PieceType.Rook => (dx == 0 || dy == 0) && IsPathClear(board, from, to),
            PieceType.Bishop => (dx == dy) && IsPathClear(board, from, to),
            PieceType.Queen => (dx == 0 || dy == 0 || dx == dy) && IsPathClear(board, from, to),
            PieceType.Pawn => (dx == 1 && dy == 1) && (from.Y - to.Y == 1),
            _ => false
        };
    }

    // 경로 장애물 확인
    private static bool IsPathClear(IReadOnlyDictionary<BoardCoord, PieceType> board, BoardCoord from, BoardCoord to)
    {
        int stepX = Math.Sign(to.X - from.X);
        int stepY = Math.Sign(to.Y - from.Y);

        int currX = from.X + stepX;
        int currY = from.Y + stepY;

        while (currX != to.X || currY != to.Y)
        {
            var checkCoord = new BoardCoord(currX, currY);
            if (board.TryGetValue(checkCoord, out var piece) && piece != PieceType.None)
                return false;

            currX += stepX;
            currY += stepY;
        }
        return true;
    }
}