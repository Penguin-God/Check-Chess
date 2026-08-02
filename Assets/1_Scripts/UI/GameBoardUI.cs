using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public GameResultUI gameResultUI;
    public Button restartBtn;

    [Header("Theme & Visuals")]
    public BoardThemeSO boardTheme; // 생성한 SO를 여기에 할당해 주세요!

    [Header("State Colors (Logic Specific)")]
    public Color activeColor = new Color(0.73f, 0.79f, 0.27f);
    public Color validMoveColor;

    [Header("Stage Data")]
    public StageDataSO currentStageData;

    GameState currentState;
    Board<UI_Square> uiSquares;

    void Start()
    {
        // 1. 임시 2차원 배열에 UI_Square를 생성하여 채웁니다.
        var uiGrid = new UI_Square[Board<UI_Square>.Size, Board<UI_Square>.Size];
        BoardIterator.DrawBoardLoop(coord =>
        {
            GameObject obj = Instantiate(squarePrefab, boardPanel);
            UI_Square squareUI = obj.GetComponent<UI_Square>();
            squareUI.Init(() => OnSquareClicked(coord.X, coord.Y));

            uiGrid[coord.X, coord.Y] = squareUI;
        });

        // 2. 완성된 배열을 기반으로 Board<UI_Square> 인스턴스를 생성합니다.
        uiSquares = new Board<UI_Square>(uiGrid);

        currentStageData = LevelManager.Instance.GetStageData(LevelManager.Instance.CurrentAbsoluteLevel);

        if (currentStageData != null)
        {
            currentState = PuzzleStageBuilder.CreateFromSO(currentStageData);
        }
        else
        {
            Debug.LogError("스테이지 데이터를 불러올 수 없습니다!");
            return;
        }

        restartBtn.onClick.AddListener(RestartStage);
        RenderState(currentState);
    }

    void OnSquareClicked(int x, int y)
    {
        var clickedCoord = new BoardCoord(x, y);
        GameState nextState = currentState;

        if (currentState.ActiveSquare == null)
        {
            nextState = ChessPuzzleLogic.SelectStartingPiece(currentState, clickedCoord);
        }
        else
        {
            nextState = ChessPuzzleLogic.MoveAndTouch(currentState, clickedCoord);

            if (nextState.IsVictory)
            {
                gameResultUI.gameObject.SetActive(true);
                gameResultUI.OnStageCleared();
            }
            else if (nextState.IsDefeat)
            {
                Debug.Log("기물이 남은 상태로 킹을 잡았습니다! 스테이지를 재시작합니다.");
                RestartStage();
            }
        }

        if (currentState != nextState)
        {
            currentState = nextState;
            RenderState(currentState);
        }
    }

    void RenderState(GameState state)
    {
        var validMoves = ChessPuzzleLogic.GetValidBatonTouches(state);

        // boardTheme에서 색상과 딕셔너리를 바로 가져옵니다.
        BoardModelMapper.CreateModel(
            state.Board,
            boardTheme.lightSquareColor,
            boardTheme.darkSquareColor,
            boardTheme.PieceSpriteDict
        ).ForEach((coord, model) =>
        {
            model = WarpModelColor(model, state, coord, validMoves);
            uiSquares[coord].UpdateVisuals(model);
        });
    }

    SquareModel WarpModelColor(SquareModel origin, GameState state, BoardCoord coord, IReadOnlyList<BoardCoord> validMoves)
    {
        Color newColor = GetStateColor(state, coord, validMoves, origin.BgColor);
        return new SquareModel(newColor, origin.Sprite);
    }

    Color GetStateColor(GameState state, BoardCoord coord, IReadOnlyList<BoardCoord> validMoves, Color baseColor)
    {
        SquareUIState uiState = SquarePresenter.DetermineSquareState(state, coord, validMoves);
        return SquarePresenter.GetStateColor(uiState, activeColor, validMoveColor, baseColor);
    }

    public void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}