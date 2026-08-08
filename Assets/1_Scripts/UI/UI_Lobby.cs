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

            // TODO : 이거 enum상태로 만들기
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
        modelBoard = modelBoard.Change(coord, modelBoard[coord] with { StatusIcon = pawnIcon } );
    }

    void Draw(BoardCoord coord, SquareModel model)
    {

    }

    void ApplySquareVisuals(Button btn, BoardCoord coord, bool canPlay, bool isCurrentlyLocked)
    {
        UI_Square squareUI = btn.GetComponent<UI_Square>();

        Color baseColor = BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);
        Color finalBgColor = canPlay ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);

        Sprite currentIcon = null;
        if (isCurrentlyLocked) // 잠겨있다면 자물쇠 스프라이트 지정
            currentIcon = lockSprite;

        SquareModel squareModel = new SquareModel(finalBgColor, currentIcon);
        squareUI.UpdateVisuals(squareModel);
    }

    void BindButtonAction(Button btn, int absoluteLevel, bool canPlay, bool isCurrentlyLocked)
    {
        btn.onClick.RemoveAllListeners();

        if (isCurrentlyLocked) btn.onClick.AddListener(() => WatchAdToUnlock(absoluteLevel));
        else if (canPlay) btn.onClick.AddListener(() => OnStageSelected(absoluteLevel));
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