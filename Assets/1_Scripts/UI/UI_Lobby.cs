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

        BoardUIHelper.DrawBoardReverseYLoop(SetupButton);
        UpdateLobbyUI();
    }

    void SetupButton(BoardCoord coord)
    {
        Button btn = Instantiate(squarePrefab, boardPanel).GetComponent<Button>();
        btn.onClick.AddListener(() => OnStageSelected(coord.X, coord.Y));
        boardButtons[coord.X, coord.Y] = btn;
    }

    //void UpdateLobbyUI()
    //{
    //    int maxCleared = LevelManager.Instance.MaxClearedLevel; // 0부터 시작

    //    // 상태 업데이트 역시 고차 함수로 깔끔하게 처리
    //    BoardUIHelper.DrawBoardLoop(coord =>
    //    {
    //        int absoluteLevel = (coord.X * BoardUIHelper.BOARD_SIZE) + coord.Y;
    //        Button btn = boardButtons[coord.X, coord.Y];

    //        // 해금 여부 판별
    //        bool isUnlocked = absoluteLevel <= maxCleared;
    //        btn.interactable = isUnlocked;

    //        // 시각적 처리 및 체스판 교차 색상 적용
    //        Image img = btn.GetComponent<Image>();
    //        if (img != null)
    //        {
    //            Color baseColor = BoardUIHelper.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);
    //            // 잠긴 스테이지는 고유의 색상에 회색조를 섞어 비활성화 느낌을 줌
    //            img.color = isUnlocked ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
    //        }

    //        // 플레이어의 현재 최고 도달 스테이지에 폰(Pawn) 배치
    //        if (absoluteLevel == maxCleared)
    //        {
    //            PlacePawnOnSquare(btn.GetComponent<RectTransform>());
    //        }
    //    });
    //}

    void UpdateLobbyUI()
    {
        int maxCleared = LevelManager.Instance.MaxClearedLevel;

        BoardUIHelper.RenderBoardVisuals(
            getPieceAt: coord =>
            {
                // TODO: 나중에 로비에 기물을 배치할 때 여기에서 PieceType을 반환하도록 수정
                return PieceType.None;
            },
            getPieceSprite: pieceType =>
            {
                // TODO: 로비 전용 스프라이트 딕셔너리가 생기면 여기서 반환
                return null;
            },
            lightColor: lightSquareColor,
            darkColor: darkSquareColor,
            applyUI: (coord, baseColor, pieceSprite, pieceColor) =>
            {
                int absoluteLevel = (coord.X * BoardUIHelper.BOARD_SIZE) + coord.Y;
                Button btn = boardButtons[coord.X, coord.Y];

                // 해금 여부 판별
                bool isUnlocked = absoluteLevel <= maxCleared;
                btn.interactable = isUnlocked;

                // 잠긴 스테이지는 회색조 적용
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    img.color = isUnlocked ? baseColor : Color.Lerp(baseColor, Color.gray, 0.7f);
                }

                // 플레이어의 현재 최고 도달 스테이지에 폰(Pawn) 배치 (기존 방식 유지)
                if (absoluteLevel == maxCleared)
                {
                    PlacePawnOnSquare(btn.GetComponent<RectTransform>());
                }
            }
        );
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