using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

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
    PieceType paintPiece = PieceType.None;
    StageEditorMode currentMode = StageEditorMode.PieceSetup;
    const int BOARD_SIZE = 8;

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

                    // 현재 선택된 모드에 따라 클릭 동작을 분기합니다.
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
        // 기물이 있다면 토글 로직 수행 (이미 설정된 곳이면 해제, 아니면 설정)
        return (getCoord() == clickCoord) ? new Vector2Int(-1, -1) : new Vector2Int(clickCoord.X, clickCoord.Y);
    }

    // --- 배경색 우선순위 ---
    Color DeterminedSquareBgColor(StageDataSO stageData, BoardCoord coord, PieceSetup currentSetup)
    {
        if (stageData.GetStartHintCoord() == coord) return Color.yellow;
        else if (stageData.GetNextHintCoord() == coord) return Color.red;
        else if (currentSetup != null && currentSetup.Piece != PieceType.None) // 일반 기물
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