using System;
using System.Collections.Generic;
using System.Linq;

public static class ChessPuzzleLogic
{
    // [1] 처음 기물 선택
    public static GameState SelectStartingPiece(GameState state, BoardCoord startSquare)
    {
        if (state.ActiveSquare != null) return state;
        if (!state.AllowedStartingSquares.Contains(startSquare)) return state;

        return state with { ActiveSquare = startSquare };
    }

    // [2] 바톤 터치가 가능한 칸 목록 반환
    public static IReadOnlyList<BoardCoord> GetValidBatonTouches(GameState state)
    {
        if (state.ActiveSquare == null) return Array.Empty<BoardCoord>();

        var active = state.ActiveSquare;

        // 딕셔너리의 KeyValuePair를 활용해 함수형 파이프라인 구성
        return state.Board
            .GetAllSquares()
            .Where(kvp => kvp.Value != PieceType.None && kvp.Key != active)
            .Select(kvp => kvp.Key)
            .Where(coord => IsValidChessMove(state.Board.ToDictionary(), active, coord))
            .ToList();
    }

    // [3] 바톤 터치 실행
    public static GameState MoveAndTouch(GameState state, BoardCoord targetSquare)
    {
        var validMoves = GetValidBatonTouches(state);
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