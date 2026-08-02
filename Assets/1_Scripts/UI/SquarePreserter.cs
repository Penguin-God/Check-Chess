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

public static class SquarePreserter
{
    public static SquareUIState DetermineSquareState(GameState state, ChessSquare square, IEnumerable<ChessSquare> validMoves)
    {
        if (state.ActiveSquare == square) return SquareUIState.Active;
        if (validMoves.Contains(square)) return SquareUIState.ValidMove;
        if (state.ActiveSquare == null && square.Piece != PieceType.None && !state.AllowedStartingSquares.Contains(square))
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
