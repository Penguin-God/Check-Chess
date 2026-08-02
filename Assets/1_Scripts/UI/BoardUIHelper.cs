using System;
using UnityEngine;

public enum BoardColorType
{
    White,
    Black,

}

public static class BoardUIHelper
{
    public static Color GetCheckerboardColor(BoardCoord boardCoord, Color lightColor, Color darkColor) => (boardCoord.X + boardCoord.Y) % 2 == 0 ? darkColor : lightColor;

    public static BoardColorType GetCheckerboardColorType(BoardCoord boardCoord) => (boardCoord.X + boardCoord.Y) % 2 == 0 ? BoardColorType.Black : BoardColorType.White;

    public static Board<BoardColorType> CreateDefaultBoard()
    {
        var grid = new BoardColorType[BOARD_SIZE, BOARD_SIZE];
        DrawBoardLoop(coord => grid[coord.X, coord.Y] = GetCheckerboardColorType(coord));
        return new Board<BoardColorType>(grid);
    }

    public const int BOARD_SIZE = 8;
    public static void DrawBoardLoop(Action<BoardCoord> onDrawSquare)
    {
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
                onDrawSquare(new BoardCoord(x, y));
        }
    }

    public static void DrawBoardReverseYLoop(Action<BoardCoord> onDrawSquare)
    {
        // 체스판의 맨 윗줄(Rank 8, index 7)부터 맨 아랫줄(Rank 1, index 0)로 내려옵니다.
        for (int y = BOARD_SIZE - 1; y >= 0; y--)
        {
            // 각 줄에서는 왼쪽(File a, index 0)부터 오른쪽(File h, index 7)으로 이동합니다.
            for (int x = 0; x < BOARD_SIZE; x++)
                onDrawSquare(new BoardCoord(x, y));
        }
    }

    public static void RenderBoardVisuals(Func<BoardCoord, PieceType> getPieceAt, Func<PieceType, Sprite> getPieceSprite, Color lightColor, Color darkColor, Action<BoardCoord, Color, Sprite, Color> applyUI)
    {
        DrawBoardLoop(coord =>
        {
            PieceType piece = getPieceAt(coord);
            Sprite sprite = null;
            Color pieceColor = Color.clear;

            if (piece != PieceType.None)
            {
                sprite = getPieceSprite(piece);
                pieceColor = sprite != null ? Color.white : Color.clear;
            }

            Color baseColor = GetCheckerboardColor(coord, lightColor, darkColor);
            applyUI(coord, baseColor, sprite, pieceColor);
        });
    }
}