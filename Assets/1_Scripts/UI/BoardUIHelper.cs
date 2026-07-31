using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BoardUIHelper
{
    public static Color GetCheckerboardColor(BoardCoord boardCoord, Color lightColor, Color darkColor) => (boardCoord.X + boardCoord.Y) % 2 == 0 ? darkColor : lightColor;

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

    public static void UpdatePieceImage(Image pieceImage, PieceType pieceType, Dictionary<PieceType, Sprite> spriteDict)
    {
        if (pieceImage == null) return;

        if (pieceType == PieceType.None)
        {
            pieceImage.sprite = null;
            pieceImage.color = Color.clear;
        }
        else
        {
            pieceImage.sprite = spriteDict[pieceType];
            pieceImage.color = Color.white;
        }
    }
}