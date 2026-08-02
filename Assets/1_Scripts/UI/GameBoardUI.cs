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
    // 기본값은 첨부해주신 이미지(image_6eea3f.png)와 유사한 체스닷컴 우드 테마 색상입니다.
    public Color lightSquareColor = new Color(0.94f, 0.85f, 0.71f);
    public Color darkSquareColor = new Color(0.71f, 0.53f, 0.39f);
    public Color activeColor = new Color(0.73f, 0.79f, 0.27f);
    public Color validMoveColor;

    GameState currentState;
    Button[,] uiButtons = new Button[8, 8];
    Image[,] uiBackgrounds = new Image[8, 8];
    Image[,] uiPieceImages = new Image[8, 8];

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
            // Note: PuzzleStageBuilder 내부에서도 반환 타입(Board)을 Dictionary로 수정하셔야 합니다!
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
        uiButtons[coord.X, coord.Y] = obj.GetComponent<Button>();
        uiBackgrounds[coord.X, coord.Y] = obj.GetComponent<Image>();
        uiPieceImages[coord.X, coord.Y] = obj.transform.GetChild(0).GetComponent<Image>();
        uiButtons[coord.X, coord.Y].onClick.AddListener(() => OnSquareClicked(coord.X, coord.Y));
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

        BoardUIHelper.RenderBoardVisuals(
            getPieceAt: coord => state.Board.TryGetValue(coord, out var piece) ? piece : PieceType.None,
            getPieceSprite: pieceType => pieceSpriteDict.TryGetValue(pieceType, out Sprite sprite) ? sprite : null,
            lightColor: lightSquareColor,
            darkColor: darkSquareColor,
            applyUI: (coord, baseColor, pieceSprite, pieceColor) =>
            {
                Image bgImg = uiBackgrounds[coord.X, coord.Y];
                Image pieceImg = uiPieceImages[coord.X, coord.Y];

                pieceImg.sprite = pieceSprite;
                pieceImg.color = pieceColor;

                SquareUIState uiState = SquarePresenter.DetermineSquareState(state, coord, validMoves);
                bgImg.color = SquarePresenter.GetStateColor(uiState, activeColor, validMoveColor, baseColor);
            }
        );
    }

    public void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}