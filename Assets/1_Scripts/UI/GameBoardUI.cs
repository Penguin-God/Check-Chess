using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct PieceSpriteData
{
    public PieceType pieceType;
    public Sprite pieceSprite;
}

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public GameResultUI gameResultUI;
    public Button restartBtn;

    [Header("Piece Sprites Configuration")]
    public List<PieceSpriteData> pieceSpritesConfig;
    Dictionary<PieceType, Sprite> pieceSpriteDict;

    [Header("Stage Data")]
    public StageDataSO currentStageData;

    [Header("Board Colors")]
    public Color lightSquareColor = new Color(0.94f, 0.85f, 0.71f);
    public Color darkSquareColor = new Color(0.71f, 0.53f, 0.39f);
    public Color activeColor = new Color(0.73f, 0.79f, 0.27f);
    public Color validMoveColor;

    GameState currentState;

    // 3개의 2차원 배열을 통합하여 UI_Square 전용 딕셔너리로 관리합니다.
    Dictionary<BoardCoord, UI_Square> uiSquares = new Dictionary<BoardCoord, UI_Square>();

    void Start()
    {
        pieceSpriteDict = new Dictionary<PieceType, Sprite>();
        foreach (var config in pieceSpritesConfig)
        {
            pieceSpriteDict[config.pieceType] = config.pieceSprite;
        }

        BoardUIHelper.DrawBoardLoop(CreateSquareUI);
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

    void CreateSquareUI(BoardCoord coord)
    {
        GameObject obj = Instantiate(squarePrefab, boardPanel);
        UI_Square squareUI = obj.GetComponent<UI_Square>();
        squareUI.Init(() => OnSquareClicked(coord.X, coord.Y));
        uiSquares[coord] = squareUI;
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
        Board<BoardColorType> defaultColorBoard = BoardUIHelper.CreateDefaultBoard();
        BoardUIHelper.DrawBoardLoop(coord =>
        {
            if (uiSquares.TryGetValue(coord, out UI_Square squareUI) == false) return;

            PieceType piece = state.Board[coord];
            Sprite pieceSprite = piece != PieceType.None && pieceSpriteDict.TryGetValue(piece, out Sprite sprite) ? sprite : null;
            Color pieceColor = pieceSprite != null ? Color.white : Color.clear;

            // 배경색 추출 (BoardColorType -> Color 변환)
            BoardColorType logicalColor = defaultColorBoard[coord];
            Color baseColor = logicalColor == BoardColorType.Black ? darkSquareColor : lightSquareColor;

            SquareUIState uiState = SquarePresenter.DetermineSquareState(state, coord, validMoves);
            Color finalBgColor = SquarePresenter.GetStateColor(uiState, activeColor, validMoveColor, baseColor);
            squareUI.UpdateVisuals(finalBgColor, pieceSprite, pieceColor);
        });
    }

    public void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
