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

    [Header("Data References")]
    [SerializeField] LockPointDataSO lockPointDataSO;
    [SerializeField] BoardThemeSO boardThemeSO; // 💡 기물 이미지를 가져오기 위해 추가된 테마 데이터

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
        var currentLocks = lockPointDataSO.GetCurrentLockPoints();

        StageCoord firstLock = currentLocks.Any() ? currentLocks.Min() : null;

        pawnMarker.gameObject.SetActive(false);
        adMarker.gameObject.SetActive(false);

        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);

            var currentState = EvaluateStageState(currentStage, maxPlayableStage);
            square.GetComponent<Button>().interactable = false;

            bool isFirstLock = (currentStage == firstLock);

            // 💡 시각적 업데이트에 coord와 maxPlayableStage를 추가로 넘겨주어 클리어 여부와 기물 판별에 사용합니다.
            ApplySquareVisuals(square, coord, currentStage, maxPlayableStage, currentState, BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor));

            BindButtonAction(square, currentStage, currentState, isFirstLock);

            // 폰 마커 배치
            if (currentStage == maxPlayableStage)
            {
                PlaceMarkerOnSquare(pawnMarker, square.GetComponent<RectTransform>());
            }

            // 첫 번째 자물쇠일 경우 광고 마커 배치
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

    void ApplySquareVisuals(UI_Square square, BoardCoord coord, StageCoord currentStage, StageCoord maxPlayableStage, StageState state, Color color)
    {
        Sprite currentIcon = null;

        if (state == StageState.LockPoint)
        {
            currentIcon = lockSprite;
        }
        else if (currentStage < maxPlayableStage) // 💡 스테이지를 '클리어'한 상태인지 확인
        {
            // 클리어한 스테이지가 8랭크 프로모션 자리라면 기물 이미지를 가져옵니다.
            PieceType? promotionPiece = GetPromotionPiece(coord);
            if (promotionPiece.HasValue)
            {
                currentIcon = boardThemeSO.PieceSpriteDict[promotionPiece.Value];
            }
        }

        SquareModel squareModel = new SquareModel(color, currentIcon);
        square.UpdateVisuals(squareModel);
    }

    // 💡 체스판의 8랭크(끝줄) 기물을 반환하는 함수
    PieceType? GetPromotionPiece(BoardCoord coord)
    {
        // BoardCoord의 y축이 7일 때 8랭크(a8~h8)라고 가정합니다. 
        // (게임 내 좌표계가 다르다면 이 부분을 y == 0 또는 다른 값으로 수정해 주세요)
        if (coord.Y != 7) return null;

        return coord.X switch
        {
            0 => PieceType.Rook,   // a8
            1 => PieceType.Knight, // b8
            2 => PieceType.Bishop, // c8
            3 => PieceType.Queen,  // d8
            4 => PieceType.King,   // e8
            5 => PieceType.Bishop, // f8
            6 => PieceType.Knight, // g8
            7 => PieceType.Rook,   // h8
            _ => null
        };
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

    void PlaceMarkerOnSquare(RectTransform marker, RectTransform squareRect, bool isTopRight = false)
    {
        if (marker == null) return;

        marker.SetParent(squareRect, false);

        if (isTopRight)
        {
            marker.anchorMin = new Vector2(1, 1);
            marker.anchorMax = new Vector2(1, 1);
            marker.pivot = new Vector2(0.5f, 0.5f);
            marker.anchoredPosition = new Vector2(-31f, -27f);
        }
        else
        {
            marker.anchorMin = new Vector2(0.5f, 0.5f);
            marker.anchorMax = new Vector2(0.5f, 0.5f);
            marker.pivot = new Vector2(0.5f, 0.5f);
            marker.anchoredPosition = Vector2.zero;
        }

        marker.gameObject.SetActive(true);
    }
}