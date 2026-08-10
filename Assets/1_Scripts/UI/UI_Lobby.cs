using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StageState
{
    LockPoint,
    Playable,
    Unplayable,
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

    Board<UI_Square> boardButtons = new();

    void Start()
    {
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI);
        UpdateLobbyUI();
    }

    void SetupSquareUI(BoardCoord coord) => boardButtons = boardButtons.Change(coord, Instantiate(squarePrefab, boardPanel).GetComponent<UI_Square>());

    void UpdateLobbyUI()
    {
        StageCoord maxPlayableStage = LocalStorage.LoadMaxPlayableStage();

        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);

            var currentState = EvaluateStageState(currentStage, maxPlayableStage);
            square.GetComponent<Button>().interactable = currentState != StageState.Unplayable;

            ApplySquareVisuals(square, BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor), currentState);
            BindButtonAction(square, currentStage, currentState);

            // 플레이어 마커(폰)를 유저가 가장 최근에 도달하여 도전할 곳에 위치시킵니다.
            if (currentStage == maxPlayableStage)
            {
                PlacePawnOnSquare(square.GetComponent<RectTransform>());
            }
        });
    }

    StageState EvaluateStageState(StageCoord stageCoord, StageCoord maxPlayableStage)
    {
        if (StageLockManager.Instance.CurrentLockPoints.Contains(stageCoord)) return StageState.LockPoint;
        else if (stageCoord > maxPlayableStage) return StageState.Unplayable;
        else return StageState.Playable;
    }

    // --- [ 부수 효과 (Side Effects) ] ---
    void ApplySquareVisuals(UI_Square square, Color color, StageState state)
    {
        Sprite currentIcon = state == StageState.LockPoint ? lockSprite : null;
        SquareModel squareModel = new SquareModel(color, currentIcon);
        square.UpdateVisuals(squareModel);
    }

    void BindButtonAction(UI_Square square, StageCoord stage, StageState state)
    {
        switch (state)
        {
            case StageState.LockPoint: square.BindClickAction(() => WatchAdToUnlock(stage)); break;
            case StageState.Playable: square.BindClickAction(() => OnStageSelected(stage)); break;
            case StageState.Unplayable: break;
        }
    }

    void WatchAdToUnlock(StageCoord stage)
    {
        Debug.Log($"챕터 {stage.ChapterIndex}, 스테이지 {stage.StageIndex} 자물쇠 해금을 위해 광고를 봅니다.");
        LevelPlayAdManager.Instance.ShowRewardedAd(() =>
        {
            StageLockManager.Instance.SaveMaxClearableStage(stage);
            UpdateLobbyUI();
        });
    }

    void OnStageSelected(StageCoord stage)
    {
        LevelManager.Instance.CurrentStage = stage;
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