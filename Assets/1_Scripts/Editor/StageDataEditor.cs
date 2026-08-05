using UnityEngine;
using UnityEditor;
using System.Linq;

// 에디터의 현재 상태를 구분하기 위한 Enum
public enum StageEditorMode
{
    PieceSetup, // 기물 배치 모드
    StartHint,  // 시작 기물 힌트(노란색) 모드
    NextHint    // 다음 기물 힌트(빨간색) 모드
}

[CustomEditor(typeof(StageDataSO))]
public class StageDataEditor : Editor
{
    private PieceType paintPiece = PieceType.None;
    private StageEditorMode currentMode = StageEditorMode.PieceSetup;

    public override void OnInspectorGUI()
    {
        StageDataSO stageData = (StageDataSO)target;

        GUILayout.Label("에디터 모드 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // EnumPopup을 사용해 3가지 모드 중 하나를 선택하도록 합니다.
        currentMode = (StageEditorMode)EditorGUILayout.EnumPopup("현재 작업 모드", currentMode);

        // 기물 배치 모드일 때만 어떤 기물을 놓을지 팝업을 띄워줍니다.
        if (currentMode == StageEditorMode.PieceSetup)
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
                string btnText = GetPieceSymbol(currentSetup?.Piece ?? PieceType.None);

                Color defaultColor = GUI.backgroundColor;

                // --- 배경색 칠하기 우선순위 ---
                if (stageData.startHintCoord.x == x && stageData.startHintCoord.y == y)
                {
                    GUI.backgroundColor = Color.yellow; // 1. 시작 힌트
                }
                else if (stageData.nextHintCoord.x == x && stageData.nextHintCoord.y == y)
                {
                    GUI.backgroundColor = Color.red;    // 2. 다음 힌트
                }
                else if (currentSetup != null && currentSetup.Piece != PieceType.None)
                {
                    GUI.backgroundColor = currentSetup.Piece == PieceType.King ? Color.gray : Color.green; // 3. 일반 기물
                }
                else
                {
                    GUI.backgroundColor = Color.white; // 4. 빈칸
                }

                if (GUILayout.Button(btnText, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Undo.RecordObject(stageData, "Edit Chess Board");

                    // 현재 선택된 모드에 따라 클릭 동작을 분기합니다.
                    switch (currentMode)
                    {
                        case StageEditorMode.StartHint:
                            // 이미 힌트인 곳을 누르면 해제, 아니면 설정
                            stageData.startHintCoord = (stageData.startHintCoord.x == x && stageData.startHintCoord.y == y)
                                ? new Vector2Int(-1, -1) : new Vector2Int(x, y);
                            break;

                        case StageEditorMode.NextHint:
                            stageData.nextHintCoord = (stageData.nextHintCoord.x == x && stageData.nextHintCoord.y == y)
                                ? new Vector2Int(-1, -1) : new Vector2Int(x, y);
                            break;

                        case StageEditorMode.PieceSetup:
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
                            break;
                    }
                    EditorUtility.SetDirty(stageData);
                }
                GUI.backgroundColor = defaultColor;
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