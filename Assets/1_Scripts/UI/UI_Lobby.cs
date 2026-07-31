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

    [Header("Board Colors")]
    public Color lightSquareColor = new Color(0.9f, 0.9f, 0.9f); // 인스펙터에서 수정 가능
    public Color darkSquareColor = new Color(0.4f, 0.6f, 0.4f);

    [Header("Premium / Ad System")]
    public GameObject adLockPanel;
    public Button watchAdButton;

    private Button[,] boardButtons = new Button[BoardUIHelper.BOARD_SIZE, BoardUIHelper.BOARD_SIZE];

    void Start()
    {
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(OnWatchAdClicked);

        BoardUIHelper.DrawBoardLoop(SetupButton);
        UpdateLobbyUI();
    }

    void SetupButton(BoardCoord coord)
    {
        Button btn = Instantiate(squarePrefab, boardPanel).GetComponent<Button>();
        btn.onClick.AddListener(() => OnStageSelected(coord.X, coord.Y));
        boardButtons[coord.X, coord.Y] = btn;
    }

    void UpdateLobbyUI()
    {
        int maxCleared = LevelManager.Instance.MaxClearedLevel; // 0부터 시작

        // 상태 업데이트 역시 고차 함수로 깔끔하게 처리
        BoardUIHelper.DrawBoardLoop(coord =>
        {
            int absoluteLevel = (coord.X * BoardUIHelper.BOARD_SIZE) + coord.Y;
            Button btn = boardButtons[coord.X, coord.Y];

            // 해금 여부 판별
            bool isUnlocked = absoluteLevel <= maxCleared;
            btn.interactable = isUnlocked;

            // 시각적 처리 및 체스판 교차 색상 적용
            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                // 헬퍼 함수를 통해 타일 고유의 밝은색/어두운색 계산
                Color baseColor = BoardUIHelper.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);

                // 잠긴 스테이지는 고유의 색상에 회색조를 섞어 비활성화 느낌을 줌
                img.color = isUnlocked ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
                print(baseColor);
            }

            // 플레이어의 현재 최고 도달 스테이지에 폰(Pawn) 배치
            if (absoluteLevel == maxCleared)
            {
                PlacePawnOnSquare(btn.GetComponent<RectTransform>());
            }
        });
    }

    void PlacePawnOnSquare(RectTransform squareRect)
    {
        if (pawnMarker == null) return;

        // 폰을 현재 활성화된 칸의 자식으로 이동시키고 중앙에 정렬
        pawnMarker.SetParent(squareRect);
        pawnMarker.anchoredPosition = Vector2.zero;
        pawnMarker.gameObject.SetActive(true);
    }

    void OnStageSelected(int x, int y)
    {
        int absoluteLevel = (x * BoardUIHelper.BOARD_SIZE) + y;
        LevelManager.Instance.CurrentAbsoluteLevel = absoluteLevel;
        SceneManager.LoadScene("Puzzle");
    }

    void OnWatchAdClicked()
    {
        LevelManager.Instance.IsPremiumUnlocked = true;
        UpdateLobbyUI();
    }
}