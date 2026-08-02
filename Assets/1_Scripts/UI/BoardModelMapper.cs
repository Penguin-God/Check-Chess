using System.Collections.Generic;
using UnityEngine;

public enum BoardColorType
{
    White,
    Black,
}

public static class BoardModelMapper
{
    public static Board<SquareModel> CreateModel(Board<PieceType> pieceBoard, Color whiteColor, Color blackColor, Dictionary<PieceType, Sprite> pieceSpriteDict)
    {
        Board<BoardColorType> defaultColorBoard = CreateDefaultBoard();
        return pieceBoard.Map((coord, piece) =>
        {
            Sprite pieceSprite = piece != PieceType.None && pieceSpriteDict.TryGetValue(piece, out Sprite sprite) ? sprite : null;
            Color baseColor = defaultColorBoard[coord] == BoardColorType.Black ? blackColor : whiteColor;
            return new SquareModel(baseColor, pieceSprite);
        });
    }

    static BoardColorType GetCheckerboardColorType(BoardCoord boardCoord) => (boardCoord.X + boardCoord.Y) % 2 == 0 ? BoardColorType.Black : BoardColorType.White;
    static Board<BoardColorType> CreateDefaultBoard() => new Board<BoardColorType>(coord => GetCheckerboardColorType(coord));
}