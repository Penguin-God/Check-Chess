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
    public RectTransform adMarker;
    public Sprite pawnIcon;
    public Sprite lockSprite;

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

        // 💡 맵을 그리기 전에 마커들을 일단 숨겨둡니다. (자물쇠가 다 열렸을 때 화면에 남는 것 방지)
        if (pawnMarker != null) pawnMarker.gameObject.SetActive(false);
        if (adMarker != null) adMarker.gameObject.SetActive(false);

        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);

            var currentState = EvaluateStageState(currentStage, maxPlayableStage);
            square.GetComponent<Button>().interactable = false;

            bool isFirstLock = (currentStage == firstLock);

            // 시각적 업데이트 (이제 합성 이미지가 필요 없으니 기본 lockSprite만 넘깁니다)
            ApplySquareVisuals(square, BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor), currentState);

            BindButtonAction(square, currentStage, currentState, isFirstLock);

            // 💡 폰 마커 배치
            if (currentStage == maxPlayableStage)
            {
                PlaceMarkerOnSquare(pawnMarker, square.GetComponent<RectTransform>());
            }

            // 💡 첫 번째 자물쇠일 경우 광고 마커 배치 (우측 상단 모서리)
            if (isFirstLock)
            {
                PlaceMarkerOnSquare(adMarker, square.GetComponent<RectTransform>(), true);
            }
        });
    }

    StageState EvaluateStageState(StageCoord stageCoord, StageCoord maxPlayableStage)
    {
        if (lockPointDataSO.GetCurrentLockPoints().Contains(stageCoord)) return StageState.LockPoint;
        else if (stageCoord > maxPlayableStage) return StageState.Unplayable;
        else return StageState.Playable;
    }

    void ApplySquareVisuals(UI_Square square, Color color, StageState state)
    {
        Sprite currentIcon = state == StageState.LockPoint ? lockSprite : null;
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

    // 💡 마커(폰, 광고 공통)를 지정된 칸에 찰싹 달라붙게 하는 범용 함수
    void PlaceMarkerOnSquare(RectTransform marker, RectTransform squareRect, bool isTopRight = false)
    {
        if (marker == null) return;

        // false를 주어 스케일이 꼬이지 않게 부모를 옮깁니다.
        marker.SetParent(squareRect, false);

        if (isTopRight)
        {
            // 🎯 [우측 상단 모서리 배치]
            // 부모(자물쇠 칸)의 우측(1) 상단(1)을 기준점으로 잡습니다.
            marker.anchorMin = new Vector2(1, 1);
            marker.anchorMax = new Vector2(1, 1);
            // 마커 자신의 중심(0.5, 0.5)을 기준점에 맞춥니다.
            marker.pivot = new Vector2(0.5f, 0.5f);

            // 모서리에서 바깥으로 살짝 튀어나오게 여백을 줍니다. (필요에 따라 숫자 조절 가능)
            marker.anchoredPosition = new Vector2(-31f, -27f);
        }
        else
        {
            // 🎯 [정중앙 배치 (폰 마커용)]
            marker.anchorMin = new Vector2(0.5f, 0.5f);
            marker.anchorMax = new Vector2(0.5f, 0.5f);
            marker.pivot = new Vector2(0.5f, 0.5f);
            marker.anchoredPosition = Vector2.zero;
        }

        marker.gameObject.SetActive(true);
    }
}