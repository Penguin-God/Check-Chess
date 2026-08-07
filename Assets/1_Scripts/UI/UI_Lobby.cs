using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    [Header("Board UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public GameObject lockIconPrefab;
    public RectTransform pawnMarker;
    public Sprite pawnIcon;

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

        // 자물쇠 아이콘 세팅 (동적으로 프리팹 생성 후 비활성화)
        Transform lockTransform = obj.transform.Find("LockIcon");
        if (lockTransform == null && lockIconPrefab != null)
        {
            GameObject lockObj = Instantiate(lockIconPrefab, obj.transform);
            lockObj.name = "LockIcon";
            lockObj.SetActive(false);
        }
        else if (lockTransform != null)
        {
            lockTransform.gameObject.SetActive(false);
        }
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

            // 1. 순수 함수로 현재 칸의 상태 데이터 연산
            bool isReached = StageLockLogic.IsReached(absoluteLevel, maxCleared);
            bool isCurrentlyLocked = StageLockLogic.IsCurrentlyLocked(absoluteLevel, lockedSet, unlockedSet);
            bool canPlay = isReached && !isCurrentlyLocked;

            // 2. 부수 효과: 시각적 상태 적용
            btn.interactable = canPlay || isCurrentlyLocked; // 플레이 가능하거나, 자물쇠를 풀 수 있거나
            ApplySquareVisuals(btn, coord, canPlay, isCurrentlyLocked);

            // 3. 버튼 동작 연결
            BindButtonAction(btn, absoluteLevel, canPlay, isCurrentlyLocked);

            // 4. 플레이어 위치(폰) 갱신
            if (absoluteLevel == maxCleared)
            {
                PlacePawnOnSquare(btn.GetComponent<RectTransform>());
            }
        });
    }

    void RenderState(BoardCoord coord)
    {
        var modelBoard = BoardModelMapper.CreateEmptyModel(boardTheme.lightSquareColor, boardTheme.darkSquareColor);
        modelBoard = modelBoard.Change(coord, modelBoard[coord] with { statusIcon = pawnIcon } );
    }

    void Draw(BoardCoord coord, SquareModel model)
    {

    }

    void ApplySquareVisuals(Button btn, BoardCoord coord, bool canPlay, bool isCurrentlyLocked)
    {
        // 자물쇠 아이콘 켜기/끄기
        Transform lockIcon = btn.transform.Find("LockIcon");
        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(isCurrentlyLocked);
        }

        // 체스판 색상 및 비활성화 음영 처리
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            Color baseColor = BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);

            // 플레이 가능한 상태가 아니면 원래 색상에 회색을 섞어 어둡게 만듭니다.
            img.color = canPlay ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
        }
    }

    void BindButtonAction(Button btn, int absoluteLevel, bool canPlay, bool isCurrentlyLocked)
    {
        // 이전 이벤트 리스너 초기화 (중복 방지)
        btn.onClick.RemoveAllListeners();

        if (isCurrentlyLocked)
        {
            // 도달하지 못했더라도 자물쇠 칸이면 언제든 눌러서 해금 시도 가능
            btn.onClick.AddListener(() => WatchAdToUnlock(absoluteLevel));
        }
        else if (canPlay)
        {
            // 자물쇠도 없고 정상적으로 도달한 칸
            btn.onClick.AddListener(() => OnStageSelected(absoluteLevel));
        }
    }

    void WatchAdToUnlock(int absoluteLevel)
    {
        Debug.Log($"레벨 {absoluteLevel} 자물쇠 해금을 위해 광고를 봅니다.");

        // 광고 매니저 호출 및 성공 시 실행될 콜백 전달
        LevelPlayAdManager.Instance.ShowRewardedAd(() =>
        {
            // 성공 시 자물쇠 해제 및 UI 즉시 갱신
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