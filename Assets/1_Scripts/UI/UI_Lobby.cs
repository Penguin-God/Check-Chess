using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    [Header("Board UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public RectTransform pawnMarker;

    [Header("Board Colors")]
    public Color lightSquareColor = new Color(0.9f, 0.9f, 0.9f);
    public Color darkSquareColor = new Color(0.4f, 0.6f, 0.4f);

    [Header("Premium / Ad System")]
    public GameObject adLockPanel;
    public Button watchAdButton;

    Button[,] boardButtons = new Button[BoardIterator.BOARD_SIZE, BoardIterator.BOARD_SIZE];

    void Start()
    {
        // 1. 이벤트 등록
        watchAdButton?.onClick.AddListener(OnWatchAdClicked);

        // 2. 초기 생성 (부수 효과)
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI);

        // 3. UI 갱신 (부수 효과)
        UpdateLobbyUI();

        void SetupSquareUI(BoardCoord coord)
        {
            Button btn = Instantiate(squarePrefab, boardPanel).GetComponent<Button>();
            btn.onClick.AddListener(() => OnStageSelected(coord));
            boardButtons[coord.X, coord.Y] = btn;
        }
    }

    void UpdateLobbyUI()
    {
        int maxCleared = LevelManager.Instance.MaxClearedLevel;

        BoardIterator.DrawBoardLoop(coord =>
        {
            int absoluteLevel = GetAbsoluteLevel(coord);
            bool isUnlocked = IsLevelUnlocked(absoluteLevel, maxCleared);
            Button btn = boardButtons[coord.X, coord.Y];

            // 데이터(순수 함수 결과)를 바탕으로 UI 상태(부수 효과) 적용
            ApplyButtonState(btn, isUnlocked);
            ApplySquareColor(btn.GetComponent<Image>(), coord, isUnlocked);

            if (IsCurrentMaxLevel(absoluteLevel, maxCleared))
            {
                PlacePawnOnSquare(btn.GetComponent<RectTransform>());
            }
        });
    }

    void ApplyButtonState(Button btn, bool isUnlocked) => btn.interactable = isUnlocked;

    void ApplySquareColor(Image img, BoardCoord coord, bool isUnlocked)
    {
        if (img == null) return;
        img.color = CalculateColor(coord, lightSquareColor, darkSquareColor, isUnlocked);
    }

    void PlacePawnOnSquare(RectTransform squareRect)
    {
        if (pawnMarker == null) return;
        pawnMarker.SetParent(squareRect);
        pawnMarker.anchoredPosition = Vector2.zero;
        pawnMarker.gameObject.SetActive(true);
    }

    void OnStageSelected(BoardCoord coord)
    {
        LevelManager.Instance.CurrentAbsoluteLevel = GetAbsoluteLevel(coord);
        SceneManager.LoadScene("Puzzle");
    }

    void OnWatchAdClicked()
    {
        LevelManager.Instance.IsPremiumUnlocked = true;
        UpdateLobbyUI(); // 상태 변경 후 즉시 렌더링 함수 재호출
    }

    // --- [ 순수 함수 (Pure Functions) 영역 ] ---
    int GetAbsoluteLevel(BoardCoord coord) => (coord.X * BoardIterator.BOARD_SIZE) + coord.Y;
    bool IsLevelUnlocked(int absoluteLevel, int maxCleared) => absoluteLevel <= maxCleared;
    bool IsCurrentMaxLevel(int absoluteLevel, int maxCleared) => absoluteLevel == maxCleared;

    Color CalculateColor(BoardCoord coord, Color light, Color dark, bool isUnlocked)
    {
        Color baseColor = BoardIterator.GetCheckerboardColor(coord, light, dark);
        return isUnlocked ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
    }
}