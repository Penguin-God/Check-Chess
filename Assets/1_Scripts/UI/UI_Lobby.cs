using System.Linq;
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
    public Sprite adLockSprite;

    [Header("Board Colors")]
    public Color lightSquareColor = new Color(0.9f, 0.9f, 0.9f);
    public Color darkSquareColor = new Color(0.4f, 0.6f, 0.4f);

    Board<UI_Square> boardButtons = new();
    [SerializeField] LockPointDataSO lockPointDataSO;

    void Start()
    {
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI);
        UpdateLobbyUI();
    }

    void SetupSquareUI(BoardCoord coord) => boardButtons = boardButtons.Change(coord, Instantiate(squarePrefab, boardPanel).GetComponent<UI_Square>());

    void UpdateLobbyUI()
    {
        StageCoord maxPlayableStage = LocalStorage.LoadMaxPlayableStage();
        var currentLocks = lockPointDataSO.GetCurrentLockPoints();

        StageCoord firstLock = currentLocks.Any() ? currentLocks.Min() : null;

        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);

            var currentState = EvaluateStageState(currentStage, maxPlayableStage);
            square.GetComponent<Button>().interactable = false;

            bool isFirstLock = (currentStage == firstLock);

            // 💡 시각적 요소를 업데이트할 때 첫 번째 자물쇠인지 여부도 같이 넘겨줍니다.
            ApplySquareVisuals(square, BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor), currentState, isFirstLock);

            BindButtonAction(square, currentStage, currentState, isFirstLock);

            if (currentStage == maxPlayableStage)
            {
                PlacePawnOnSquare(square.GetComponent<RectTransform>());
            }
        });
    }

    StageState EvaluateStageState(StageCoord stageCoord, StageCoord maxPlayableStage)
    {
        if (lockPointDataSO.GetCurrentLockPoints().Contains(stageCoord)) return StageState.LockPoint;
        else if (stageCoord > maxPlayableStage) return StageState.Unplayable;
        else return StageState.Playable;
    }

    // 💡 매개변수에 isFirstLock을 추가하여 스프라이트를 분기합니다.
    void ApplySquareVisuals(UI_Square square, Color color, StageState state, bool isFirstLock)
    {
        Sprite currentIcon = null;

        if (state == StageState.LockPoint)
        {
            // 첫 번째 자물쇠면 광고 자물쇠 이미지를, 아니면 일반 자물쇠 이미지를 넣습니다.
            currentIcon = isFirstLock ? adLockSprite : lockSprite;
        }

        SquareModel squareModel = new SquareModel(color, currentIcon);
        square.UpdateVisuals(squareModel);
    }

    void BindButtonAction(UI_Square square, StageCoord stage, StageState state, bool isFirstLock)
    {
        switch (state)
        {
            case StageState.LockPoint:
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
        AdManager.Instance.ShowRewardedAd(() =>
        {
            lockPointDataSO.SaveMaxClearableStage(stage);
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