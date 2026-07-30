using System;
using UnityEngine;

public static class BoardUIHelper
{
    public static Color GetCheckerboardColor(BoardCoord boardCoord, Color lightColor, Color darkColor) => (boardCoord.X + boardCoord.Y) % 2 == 0 ? darkColor : lightColor;

    public const int BOARD_SIZE = 8;
    public static void DrawBoard(Action<BoardCoord> onDrawSquare)
    {
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                onDrawSquare(new BoardCoord(x, y));
            }
        }
    }
}