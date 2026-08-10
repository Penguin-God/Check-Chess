using System.Linq; // [추가] Any()와 Min()을 사용하기 위해 필요합니다.
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
    [SerializeField] StageLockManager stageLockManager;
    void Start()
    {
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI);
        UpdateLobbyUI();
    }

    void SetupSquareUI(BoardCoord coord) => boardButtons = boardButtons.Change(coord, Instantiate(squarePrefab, boardPanel).GetComponent<UI_Square>());

    void UpdateLobbyUI()
    {
        StageCoord maxPlayableStage = LocalStorage.LoadMaxPlayableStage();
        var currentLocks = stageLockManager.CurrentLockPoints;

        // [핵심] 현재 남은 자물쇠 집합 중 가장 작은 값(첫 번째 자물쇠)을 찾습니다!
        // (StageCoord에 IComparable을 구현해 두었기 때문에 .Min()이 완벽하게 작동합니다)
        StageCoord firstLock = currentLocks.Any() ? currentLocks.Min() : null;

        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);

            var currentState = EvaluateStageState(currentStage, maxPlayableStage);
            square.GetComponent<Button>().interactable = false;

            ApplySquareVisuals(square, BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor), currentState);

            // 바인딩 시에도 첫 번째 자물쇠 여부를 함께 넘겨줍니다.
            BindButtonAction(square, currentStage, currentState, currentStage == firstLock);

            if (currentStage == maxPlayableStage)
            {
                PlacePawnOnSquare(square.GetComponent<RectTransform>());
            }
        });
    }

    StageState EvaluateStageState(StageCoord stageCoord, StageCoord maxPlayableStage)
    {
        if (stageLockManager.CurrentLockPoints.Contains(stageCoord)) return StageState.LockPoint;
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

    // 매개변수로 isFirstLock을 추가하여 로직을 더 안전하게 제어합니다.
    void BindButtonAction(UI_Square square, StageCoord stage, StageState state, bool isFirstLock)
    {
        switch (state)
        {
            case StageState.LockPoint:
                // 오직 첫 번째 자물쇠일 때만 광고 이벤트를 바인딩합니다.
                if (isFirstLock)
                    square.BindClickAction(() => WatchAdToUnlock(stage));
                break;
            case StageState.Playable:
                square.BindClickAction(() => OnStageSelected(stage));
                break;
            case StageState.Unplayable:
                break;
        }
    }

    void WatchAdToUnlock(StageCoord stage)
    {
        Debug.Log($"챕터 {stage.ChapterIndex}, 스테이지 {stage.StageIndex} 자물쇠 해금을 위해 광고를 봅니다.");
        LevelPlayAdManager.Instance.ShowRewardedAd(() =>
        {
            stageLockManager.SaveMaxClearableStage(stage);
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