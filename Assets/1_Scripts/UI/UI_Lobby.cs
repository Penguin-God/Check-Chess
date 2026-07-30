using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Lobby : MonoBehaviour
{
    [Header("Board UI References")]
    public Transform boardPanel;    // GridLayoutGroup이 적용된 8x8 패널
    public GameObject squarePrefab; // 개별 칸(스테이지) 프리팹
    public RectTransform pawnMarker; // 플레이어의 현재 위치를 표시할 폰 이미지 UI

    [Header("Premium / Ad System")]
    public GameObject adLockPanel;
    public Button watchAdButton;

    const int CHESS_LIEN_COUNT = 8;
    // 8x8 배열 (x: File/챕터, y: Rank/스테이지)
    private Button[,] boardButtons = new Button[CHESS_LIEN_COUNT, CHESS_LIEN_COUNT];

    void Start()
    {
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(OnWatchAdClicked);

        InitializeChessBoard();
        UpdateLobbyUI();
    }

    void InitializeChessBoard()
    {
        // 체스판은 보통 왼쪽 아래가 a1이므로, UI 생성 시 맨 윗줄(Rank 8)부터 아래(Rank 1)로 내려오며 생성합니다.
        for (int rank = 7; rank >= 0; rank--)
        {
            for (int file = 0; file < 8; file++)
            {
                int f = file; // a ~ h (챕터)
                int r = rank; // 1 ~ 8 (스테이지)

                GameObject obj = Instantiate(squarePrefab, boardPanel);
                Button btn = obj.GetComponent<Button>();

                // a1, b2 형태의 네이밍 적용
                char fileChar = (char)('a' + f);
                int rankNum = r + 1;

                TMP_Text label = obj.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = $"{fileChar}{rankNum}";

                btn.onClick.AddListener(() => OnStageSelected(f, r));
                boardButtons[f, r] = btn;
            }
        }
    }

    void UpdateLobbyUI()
    {
        int maxCleared = LevelManager.Instance.MaxClearedLevel; // 0부터 시작

        for (int file = 0; file < CHESS_LIEN_COUNT; file++)
        {
            for (int rank = 0; rank < CHESS_LIEN_COUNT; rank++)
            {
                int absoluteLevel = (file * CHESS_LIEN_COUNT) + rank;
                Button btn = boardButtons[file, rank];

                // 해금 여부 판별
                bool isUnlocked = absoluteLevel <= maxCleared;
                btn.interactable = isUnlocked;

                // 잠긴 스테이지 시각적 처리 (회색조)
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    img.color = isUnlocked ? Color.white : new Color(0.7f, 0.7f, 0.7f);
                }

                // 플레이어의 현재 최고 도달 스테이지에 폰(Pawn) 배치
                if (absoluteLevel == maxCleared)
                {
                    PlacePawnOnSquare(btn.GetComponent<RectTransform>());
                }
            }
        }
    }

    void PlacePawnOnSquare(RectTransform squareRect)
    {
        if (pawnMarker == null) return;

        // 폰을 현재 활성화된 칸의 자식으로 이동시키고 중앙에 정렬
        pawnMarker.SetParent(squareRect);
        pawnMarker.anchoredPosition = Vector2.zero;
        pawnMarker.gameObject.SetActive(true);
    }

    void OnStageSelected(int file, int rank)
    {
        int absoluteLevel = (file * CHESS_LIEN_COUNT) + rank;
        LevelManager.Instance.CurrentAbsoluteLevel = absoluteLevel;
        SceneManager.LoadScene("Puzzle");
    }

    void OnWatchAdClicked()
    {
        LevelManager.Instance.IsPremiumUnlocked = true;
        UpdateLobbyUI();
    }
}