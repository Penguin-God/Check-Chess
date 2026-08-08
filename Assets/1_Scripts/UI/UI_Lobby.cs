using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StageState
{
    Locked,
    Playable,
    Unreached
}

public class UI_Lobby : MonoBehaviour
{
    [Header("Board UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public RectTransform pawnMarker;
    public Sprite pawnIcon;
    public Sprite lockSprite;

    [Header("Board Colors")]
    public Color lightSquareColor = new Color(0.9f, 0.9f, 0.9f);
    public Color darkSquareColor = new Color(0.4f, 0.6f, 0.4f);

    BoardThemeSO boardTheme;
    Board<UI_Square> boardButtons = new();

    void Start()
    {
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI);
        UpdateLobbyUI();
    }

    void SetupSquareUI(BoardCoord coord)
    {
        GameObject obj = Instantiate(squarePrefab, boardPanel);
        boardButtons = boardButtons.Change(coord, obj.GetComponent<UI_Square>());
    }

    void UpdateLobbyUI()
    {
        // 1. 최고 도달 레벨도 StageCoord 레코드로 변환합니다.
        StageCoord maxClearedStage = StageCoord.FromAbsoluteLevel(LevelManager.Instance.MaxClearedLevel);
        var lockedSet = StageLockManager.Instance.DesignatedLockLevels;
        var unlockedSet = StageLockManager.Instance.UnlockedLevels;

        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);
            StageState currentState = EvaluateStageState(currentStage, maxClearedStage, lockedSet, unlockedSet);

            square.GetComponent<Button>().interactable = currentState != StageState.Unreached;
            ApplySquareVisuals(square, coord, currentState);
            BindButtonAction(square, currentStage, currentState);

            if (currentStage == maxClearedStage)
            {
                PlacePawnOnSquare(square.GetComponent<RectTransform>());
            }
        });
    }

    StageState EvaluateStageState(StageCoord currentStage, StageCoord maxClearedStage, HashSet<int> lockedSet, HashSet<int> unlockedSet)
    {
        if (StageLogic.IsCurrentlyLocked(currentStage.ToAbsoluteLevel(), lockedSet, unlockedSet)) return StageState.Locked;
        if (currentStage <= maxClearedStage) return StageState.Playable;

        return StageState.Unreached;
    }

    void RenderState(BoardCoord coord)
    {
        var modelBoard = BoardModelMapper.CreateEmptyModel(boardTheme.lightSquareColor, boardTheme.darkSquareColor);
        modelBoard = modelBoard.Change(coord, modelBoard[coord] with { StatusIcon = pawnIcon });
    }

    void Draw(BoardCoord coord, SquareModel model)
    {
    }

    // --- [ 부수 효과 (Side Effects) ] ---

    void ApplySquareVisuals(UI_Square square, BoardCoord coord, StageState state)
    {
        Color baseColor = BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);
        Color finalBgColor = state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
        Sprite currentIcon = state == StageState.Locked ? lockSprite : null;

        SquareModel squareModel = new SquareModel(finalBgColor, currentIcon);
        square.UpdateVisuals(squareModel);
    }

    // 절대 레벨 숫자 대신 StageCoord 객체를 받도록 변경
    void BindButtonAction(UI_Square square, StageCoord stage, StageState state)
    {
        switch (state)
        {
            case StageState.Locked: square.BindClickAction(() => WatchAdToUnlock(stage)); break;
            case StageState.Playable: square.BindClickAction(() => OnStageSelected(stage)); break;
            case StageState.Unreached: break;
        }
    }

    void WatchAdToUnlock(StageCoord stage)
    {
        // 레코드를 쓰면 로그를 찍을 때도 챕터와 스테이지를 분리해서 보기 훨씬 편해집니다!
        Debug.Log($"챕터 {stage.ChapterIndex}, 스테이지 {stage.StageIndex} 자물쇠 해금을 위해 광고를 봅니다.");

        LevelPlayAdManager.Instance.ShowRewardedAd(() =>
        {
            // 실제 데이터 저장소(StageLockManager)에 넘길 때만 숫자로 변환
            StageLockManager.Instance.UnlockLevel(stage.ToAbsoluteLevel());
            UpdateLobbyUI();
        });
    }

    void OnStageSelected(StageCoord stage)
    {
        // 씬을 넘어가기 전 LevelManager에 저장할 때만 숫자로 변환
        LevelManager.Instance.CurrentAbsoluteLevel = stage.ToAbsoluteLevel();
        SceneManager.LoadScene("Puzzle");
    }

    void PlacePawnOnSquare(RectTransform squareRect)
    {
        if (pawnMarker == null) return;
        pawnMarker.SetParent(squareRect);
        pawnMarker.anchoredPosition = Vector2.zero;
        pawnMarker.gameObject.SetActive(true);
    }
}