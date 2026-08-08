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
    Board<Button> boardButtons = new();

    void Start()
    {
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI);
        UpdateLobbyUI();
    }

    void SetupSquareUI(BoardCoord coord)
    {
        GameObject obj = Instantiate(squarePrefab, boardPanel);
        boardButtons = boardButtons.Change(coord, obj.GetComponent<Button>());
    }

    void UpdateLobbyUI()
    {
        int maxCleared = LevelManager.Instance.MaxClearedLevel;
        var lockedSet = StageLockManager.Instance.DesignatedLockLevels;
        var unlockedSet = StageLockManager.Instance.UnlockedLevels;

        BoardIterator.DrawBoardLoop(coord =>
        {
            int absoluteLevel = (coord.X * BoardIterator.BOARD_SIZE) + coord.Y;
            Button btn = boardButtons[coord];

            // 1. 순수 함수로 현재 칸의 명확한 '상태(State)' 하나를 도출
            StageState currentState = EvaluateStageState(absoluteLevel, maxCleared, lockedSet, unlockedSet);
            btn.interactable = currentState != StageState.Unreached; // Unreached가 아닐 때만 터치 가능
            ApplySquareVisuals(btn, coord, currentState);

            // 3. 버튼 동작 연결
            BindButtonAction(btn, absoluteLevel, currentState);

            // 4. 플레이어 위치(폰) 갱신
            if (absoluteLevel == maxCleared)
            {
                PlacePawnOnSquare(btn.GetComponent<RectTransform>());
            }
        });
    }

    StageState EvaluateStageState(int absoluteLevel, int maxCleared, HashSet<int> lockedSet, HashSet<int> unlockedSet)
    {
        if (StageLockLogic.IsCurrentlyLocked(absoluteLevel, lockedSet, unlockedSet)) return StageState.Locked;
        if (StageLockLogic.IsReached(absoluteLevel, maxCleared)) return StageState.Playable;
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

    void ApplySquareVisuals(Button btn, BoardCoord coord, StageState state)
    {
        UI_Square squareUI = btn.GetComponent<UI_Square>();
        Color baseColor = BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);
        Color finalBgColor = state == StageState.Playable ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
        Sprite currentIcon = state == StageState.Locked ? lockSprite : null;

        SquareModel squareModel = new SquareModel(finalBgColor, currentIcon);
        squareUI.UpdateVisuals(squareModel);
    }

    void BindButtonAction(Button btn, int absoluteLevel, StageState state)
    {
        btn.onClick.RemoveAllListeners();

        switch (state)
        {
            case StageState.Locked: btn.onClick.AddListener(() => WatchAdToUnlock(absoluteLevel)); break;
            case StageState.Playable: btn.onClick.AddListener(() => OnStageSelected(absoluteLevel)); break;
            case StageState.Unreached: break;
        }
    }

    void WatchAdToUnlock(int absoluteLevel)
    {
        Debug.Log($"레벨 {absoluteLevel} 자물쇠 해금을 위해 광고를 봅니다.");

        LevelPlayAdManager.Instance.ShowRewardedAd(() =>
        {
            StageLockManager.Instance.UnlockLevel(absoluteLevel);
            UpdateLobbyUI();
        });
    }

    void OnStageSelected(int absoluteLevel)
    {
        LevelManager.Instance.CurrentAbsoluteLevel = absoluteLevel;
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