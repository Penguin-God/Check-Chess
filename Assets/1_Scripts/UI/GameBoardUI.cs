using System;
using System.Collections.Generic;
using System.Linq;
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

        InitializeUI();
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

    void InitializeUI()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int clickX = x;
                int clickY = y;
                GameObject obj = Instantiate(squarePrefab, boardPanel);

                uiButtons[x, y] = obj.GetComponent<Button>();
                uiBackgrounds[x, y] = obj.GetComponent<Image>();
                uiPieceImages[x, y] = obj.transform.GetChild(0).GetComponent<Image>();

                uiButtons[x, y].onClick.AddListener(() => OnSquareClicked(clickX, clickY));
            }
        }
    }

    void OnSquareClicked(int x, int y)
    {
        var clickedSquare = currentState.Board.FirstOrDefault(sq => sq.X == x && sq.Y == y);
        if (clickedSquare == null) return;

        GameState nextState = currentState;

        if (currentState.ActiveSquare == null)
        {
            nextState = ChessPuzzleLogic.SelectStartingPiece(currentState, clickedSquare);
        }
        else
        {
            nextState = ChessPuzzleLogic.MoveAndTouch(currentState, clickedSquare);

            // 1. 성공 조건: 기물을 모두 소진하고 킹을 잡음
            if (nextState.IsVictory)
            {
                gameResultUI.gameObject.SetActive(true);
                gameResultUI.OnStageCleared();
            }
            // 2. 실패 조건: 기물이 남았는데 킹을 먼저 잡음
            else if (nextState.IsDefeat)
            {
                Debug.Log("기물이 남은 상태로 킹을 잡았습니다! 스테이지를 재시작합니다.");
                RestartStage(); // 실패 시 즉시 현재 스테이지를 재시작합니다
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
        BoardUIHelper.DrawBoardLoop(coord => UpdateSquareUI(coord, state, validMoves));
    }

    private void UpdateSquareUI(BoardCoord coord, GameState state, IEnumerable<ChessSquare> validMoves)
    {
        // 1. 순수 좌표(coord)를 이용해 게임 상태(state)에서 해당 칸의 상태(ChessSquare)를 찾음
        var square = state.Board.FirstOrDefault(sq => sq.X == coord.X && sq.Y == coord.Y);

        // 해당하는 칸의 데이터가 없다면 그리지 않음
        if (square == null) return;

        Image bgImg = uiBackgrounds[coord.X, coord.Y];
        Image pieceImg = uiPieceImages[coord.X, coord.Y];

        // 2. 기물 이미지 렌더링 (square.Piece 사용)
        if (square.Piece == PieceType.None)
        {
            pieceImg.sprite = null;
            pieceImg.color = Color.clear;
        }
        else
        {
            if (pieceSpriteDict.TryGetValue(square.Piece, out Sprite sprite))
            {
                pieceImg.sprite = sprite;
                pieceImg.color = Color.white;
            }
            else
            {
                pieceImg.sprite = null;
                pieceImg.color = Color.clear;
            }
        }

        // 3. 타일 기본 색상 계산 (coord 사용)
        Color baseColor = BoardUIHelper.GetCheckerboardColor(coord, lightSquareColor, darkSquareColor);

        // 4. 상태값 판별 (square 사용)
        bool isActive = state.ActiveSquare == square;
        bool isValidMove = validMoves.Contains(square);
        bool isStartDisabled = state.ActiveSquare == null &&
                               square.Piece != PieceType.None &&
                               !state.AllowedStartingSquares.Contains(square);

        // 5. 배경색 렌더링 적용
        if (isActive)
        {
            bgImg.color = activeColor;
        }
        else if (isValidMove)
        {
            bgImg.color = validMoveColor;
        }
        else if (isStartDisabled)
        {
            bgImg.color = Color.Lerp(baseColor, Color.gray, 0.6f);
        }
        else
        {
            bgImg.color = baseColor;
        }
    }
    public void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}