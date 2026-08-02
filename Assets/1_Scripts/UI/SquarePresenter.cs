using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SquareUIState
{
    Normal,
    Active,
    ValidMove,
    StartDisabled
}

public static class SquarePresenter
{
    public static SquareUIState DetermineSquareState(GameState state, BoardCoord coord, IEnumerable<BoardCoord> validMoves)
    {
        if (state.ActiveSquare == coord) return SquareUIState.Active;
        if (validMoves.Contains(coord)) return SquareUIState.ValidMove;

        state.Board.ToDictionary().TryGetValue(coord, out var piece);

        // 아무 기물도 선택되지 않은 초기 상태일 때, 기물이 King이라면 비활성화 표시
        if (state.ActiveSquare == null && piece == PieceType.King)
            return SquareUIState.StartDisabled;

        return SquareUIState.Normal;
    }

    public static Color GetStateColor(SquareUIState uiState, Color activeColor, Color validMoveColor, Color baseColor) => uiState switch
    {
        SquareUIState.Active => activeColor,
        SquareUIState.ValidMove => validMoveColor,
        SquareUIState.StartDisabled => Color.Lerp(baseColor, Color.gray, 0.6f),
        SquareUIState.Normal => baseColor,
        _ => baseColor
    };
}