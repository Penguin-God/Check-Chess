using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public enum StageEditorMode
{
    PieceSetup,
    StartHint,
    NextHint
}

[CustomEditor(typeof(StageDataSO))]
public class StageDataEditor : Editor
{
    PieceType paintPiece = PieceType.None;
    StageEditorMode currentMode = StageEditorMode.PieceSetup;
    const int BOARD_SIZE = 8;

    public override void OnInspectorGUI()
    {
        StageDataSO stageData = (StageDataSO)target;
        serializedObject.Update();

        // StageText 프로퍼티만 찾아서 화면에 예쁘게 그립니다
        SerializedProperty stageTextProp = serializedObject.FindProperty("StageText");
        EditorGUILayout.PropertyField(stageTextProp, new GUIContent("튜토리얼 텍스트"));

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(15); // 그리드와의 간격 벌리기

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

        // ... (이하 보드 그리는 2중 for문 코드는 기존과 동일) ...
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                var coord = new BoardCoord(x, y);
                PieceSetup currentSetup = stageData.initialPieces.FirstOrDefault(p => p.X == x && p.Y == y);
                string btnText = GetPieceSymbol(currentSetup?.Piece ?? PieceType.None);

                Color originColor = GUI.backgroundColor;
                GUI.backgroundColor = DeterminedSquareBgColor(stageData, coord, currentSetup);

                if (GUILayout.Button(btnText, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Undo.RecordObject(stageData, "Edit Chess Board");

                    switch (currentMode)
                    {
                        case StageEditorMode.StartHint: stageData.startHintCoord = TryToggleHintCoord(stageData.GetStartHintCoord, coord, currentSetup); break;
                        case StageEditorMode.NextHint: stageData.nextHintCoord = TryToggleHintCoord(stageData.GetNextHintCoord, coord, currentSetup); break;

                        case StageEditorMode.PieceSetup:
                            if (paintPiece == PieceType.None && currentSetup != null) stageData.initialPieces.Remove(currentSetup);
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
                GUI.backgroundColor = originColor;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    Vector2Int TryToggleHintCoord(Func<BoardCoord> getCoord, BoardCoord clickCoord, PieceSetup currentSetup)
    {
        if (currentSetup == null || currentSetup.Piece == PieceType.None) return new Vector2Int(-1, -1);
        return (getCoord() == clickCoord) ? new Vector2Int(-1, -1) : new Vector2Int(clickCoord.X, clickCoord.Y);
    }

    Color DeterminedSquareBgColor(StageDataSO stageData, BoardCoord coord, PieceSetup currentSetup)
    {
        if (stageData.GetStartHintCoord() == coord) return Color.yellow;
        else if (stageData.GetNextHintCoord() == coord) return Color.red;
        else if (currentSetup != null && currentSetup.Piece != PieceType.None)
            return currentSetup.Piece == PieceType.King ? Color.gray : Color.green;
        else return Color.white;
    }

    string GetPieceSymbol(PieceType type)
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