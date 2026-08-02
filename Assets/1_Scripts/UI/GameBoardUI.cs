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
    Board<UI_Square> uiSquares;

    void Start()
    {
        pieceSpriteDict = new Dictionary<PieceType, Sprite>();
        foreach (var config in pieceSpritesConfig)
        {
            pieceSpriteDict[config.pieceType] = config.pieceSprite;
        }

        // 1. 임시 2차원 배열에 UI_Square를 생성하여 채웁니다.
        var uiGrid = new UI_Square[Board<UI_Square>.Size, Board<UI_Square>.Size];
        BoardUIHelper.DrawBoardLoop(coord =>
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
        Board<BoardColorType> defaultColorBoard = BoardUIHelper.CreateDefaultBoard();
        MyClass.CreateModel(state.Board, lightSquareColor, darkSquareColor, pieceSpriteDict).ForEach((coord, model) =>
        {
            model = WarpModelColor(model, state, coord, validMoves);
            uiSquares[coord].UpdateVisuals(model);
        });
        //uiSquares.ForEach((coord, squareUI) =>
        //{
        //    var model = CreateModel(state, coord, defaultColorBoard);
        //    model = WarpModelColor(model, state, coord, validMoves);
        //    squareUI.UpdateVisuals(model);
        //});
    }

    SquareModel CreateModel(GameState state, BoardCoord coord, Board<BoardColorType> defaultColorBoard)
    {
        PieceType piece = state.Board[coord];
        Sprite pieceSprite = piece != PieceType.None && pieceSpriteDict.TryGetValue(piece, out Sprite sprite) ? sprite : null;

        BoardColorType logicalColor = defaultColorBoard[coord];
        Color baseColor = logicalColor == BoardColorType.Black ? darkSquareColor : lightSquareColor;

        return new SquareModel(baseColor, pieceSprite);
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

public static class MyClass
{
    public static Board<SquareModel> CreateModel(Board<PieceType> pieceBoard, Color whiteColor, Color blackColor, Dictionary<PieceType, Sprite> pieceSpriteDict)
    {
        Board<BoardColorType> defaultColorBoard = BoardUIHelper.CreateDefaultBoard();
        return pieceBoard.Map((coord, piece) =>
        {
            Sprite pieceSprite = piece != PieceType.None && pieceSpriteDict.TryGetValue(piece, out Sprite sprite) ? sprite : null;
            Color baseColor = defaultColorBoard[coord] == BoardColorType.Black ? blackColor : whiteColor;
            return new SquareModel(baseColor, pieceSprite);
        });
    }
}