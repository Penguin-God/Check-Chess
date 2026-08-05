using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(StageDataSO))]
public class StageDataEditor : Editor
{
    private PieceType paintPiece = PieceType.None;
    private bool isHintMode = false; // 힌트 설정 모드용 토글

    public override void OnInspectorGUI()
    {
        StageDataSO stageData = (StageDataSO)target;

        GUILayout.Label("에디터 모드 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        isHintMode = EditorGUILayout.Toggle("힌트 지정 모드", isHintMode);

        // 힌트 모드가 아닐 때만 기물 선택 팝업을 보여줍니다.
        if (!isHintMode)
        {
            paintPiece = (PieceType)EditorGUILayout.EnumPopup("배치할 기물", paintPiece);
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(15);
        GUILayout.Label("보드 (8x8 Grid)", EditorStyles.boldLabel);

        for (int y = 0; y < 8; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 8; x++)
            {
                PieceSetup currentSetup = stageData.initialPieces.FirstOrDefault(p => p.X == x && p.Y == y);

                // 글자는 현재 설정된 기물의 심볼을 그대로 가져옵니다.
                string btnText = GetPieceSymbol(currentSetup?.Piece ?? PieceType.None);

                Color defaultColor = GUI.backgroundColor;

                // 1순위: 해당 좌표가 힌트로 설정되어 있다면 무조건 배경을 노란색으로 칠합니다.
                if (stageData.hintCoord.x == x && stageData.hintCoord.y == y)
                {
                    GUI.backgroundColor = Color.yellow;
                }
                // 2순위: 힌트가 아니고 기물이 존재한다면 기존 로직대로 칠합니다.
                else if (currentSetup != null && currentSetup.Piece != PieceType.None)
                {
                    GUI.backgroundColor = currentSetup.Piece == PieceType.King ? Color.gray : Color.green;
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                if (GUILayout.Button(btnText, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Undo.RecordObject(stageData, "Edit Chess Board");

                    if (isHintMode)
                    {
                        // 힌트 모드일 때 클릭: 이미 힌트인 곳을 클릭하면 해제(-1, -1), 아니면 지정
                        if (stageData.hintCoord.x == x && stageData.hintCoord.y == y)
                            stageData.hintCoord = new Vector2Int(-1, -1);
                        else
                            stageData.hintCoord = new Vector2Int(x, y);
                    }
                    else
                    {
                        // 기존 기물 배치 모드 로직
                        if (paintPiece == PieceType.None)
                        {
                            if (currentSetup != null) stageData.initialPieces.Remove(currentSetup);
                        }
                        else
                        {
                            if (currentSetup == null)
                            {
                                currentSetup = new PieceSetup { X = x, Y = y };
                                stageData.initialPieces.Add(currentSetup);
                            }
                            currentSetup.Piece = paintPiece;
                        }
                    }
                    EditorUtility.SetDirty(stageData);
                }
                GUI.backgroundColor = defaultColor; // 버튼을 그린 후엔 색상 복구
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private string GetPieceSymbol(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => "P",
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            PieceType.Rook => "R",
            PieceType.Queen => "Q",
            PieceType.King => "K",
            _ => ""
        };
    }
}