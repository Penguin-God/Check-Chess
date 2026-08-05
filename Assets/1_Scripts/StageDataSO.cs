using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PieceSetup
{
    public int X;
    public int Y;
    public PieceType Piece;
}

[CreateAssetMenu(fileName = "NewStage", menuName = "ChessPuzzle/StageData")]
public class StageDataSO : ScriptableObject
{
    [Header("스테이지에 배치할 기물들")]
    public List<PieceSetup> initialPieces = new List<PieceSetup>();

    [Header("힌트 좌표 (에디터 클릭으로 설정됨)")]
    // -1, -1은 힌트가 아직 설정되지 않았음을 의미합니다.
    public Vector2Int hintCoord = new Vector2Int(-1, -1);

    public BoardCoord GetHintCoord()
    {
        if (hintCoord.x < 0 || hintCoord.y < 0)return null;
        return new BoardCoord(hintCoord.x, hintCoord.y);
    }
}