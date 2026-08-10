using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    void Start()
    {
        BoardIterator.DrawBoardReverseYLoop(SetupSquareUI); // Grid 규칙 때문에 역으로 설정해야 정좌표가 됨
        UpdateLobbyUI();
    }

    void SetupSquareUI(BoardCoord coord) => boardButtons = boardButtons.Change(coord, Instantiate(squarePrefab, boardPanel).GetComponent<UI_Square>());

    void UpdateLobbyUI()
    {
        StageCoord maxClearedStage = LocalStorage.LoadMaxClearedStage();
        
        BoardIterator.DrawBoardLoop(coord =>
        {
            UI_Square square = boardButtons[coord];
            StageCoord currentStage = StageCoord.FromBoardCoord(coord);
            StageState currentState = StageLockManager.Instance.EvaluateStageState(currentStage);

            square.GetComponent<Button>().interactable = currentState != StageState.Unreached;
            ApplySquareVisuals(square, StageStatusLogic.GetStatusColor(currentState, BoardIterator.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor)), currentState);
            BindButtonAction(square, currentStage, currentState);

            if (currentStage == maxClearedStage)
            {
                PlacePawnOnSquare(square.GetComponent<RectTransform>());
            }
        });
    }


    // --- [ 부수 효과 (Side Effects) ] ---

    void ApplySquareVisuals(UI_Square square, Color color, StageState state)
    {
        Sprite currentIcon = state == StageState.Locked ? lockSprite : null;
        SquareModel squareModel = new SquareModel(color, currentIcon);
        square.UpdateVisuals(squareModel);
    }

    // 절대 레벨 숫자 대신 StageCoord 객체를 받도록 변경
    void BindButtonAction(UI_Square square, StageCoord stage, StageState state)
    {
        switch (state)
        {
            case StageState.Locked: square.BindClickAction(() => WatchAdToUnlock(stage)); break;
            case StageState.Playable: square.BindClickAction(() => OnStageSelected(stage)); break;
            case StageState.Unreached: break;
        }
    }

    void WatchAdToUnlock(StageCoord stage)
    {
        Debug.Log($"챕터 {stage.ChapterIndex}, 스테이지 {stage.StageIndex} 자물쇠 해금을 위해 광고를 봅니다.");

        //LevelPlayAdManager.Instance.ShowRewardedAd(() =>
        //{
        //    StageLockManager.Instance.UnlockLevel(stage);
        //    UpdateLobbyUI();
        //});

        //StageLockManager.Instance.UnlockLevel(stage);
        StageLockManager.Instance._UnlockLevel(stage);
        UpdateLobbyUI();
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


