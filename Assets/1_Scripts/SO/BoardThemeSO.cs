using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct PieceSpriteData
{
    public PieceType pieceType;
    public Sprite pieceSprite;
}

[CreateAssetMenu(fileName = "NewBoardTheme", menuName = "ChessPuzzle/Board Theme")]
public class BoardThemeSO : ScriptableObject
{
    [Header("Board Colors")]
    public Color lightSquareColor = new Color(0.94f, 0.85f, 0.71f);
    public Color darkSquareColor = new Color(0.71f, 0.53f, 0.39f);

    [Header("Piece Sprites Configuration")]
    [SerializeField] private List<PieceSpriteData> pieceSpritesConfig;

    public Dictionary<PieceType, Sprite> PieceSpriteDict => pieceSpritesConfig.ToDictionary(x => x.pieceType, x => x.pieceSprite);
}