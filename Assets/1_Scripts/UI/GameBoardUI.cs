using System.Collections.Generic;
using UnityEngine;

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public UI_ButtonPanel buttonPanel;

    [Header("Theme & Visuals")]
    public BoardThemeSO boardTheme;

    [Header("State Colors (Logic Specific)")]
    public Color activeColor = new Color(0.73f, 0.79f, 0.27f);
    public Color validMoveColor;

    [Header("Stage Data")]
    public StageDataSO currentStageData;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip captureSound;
    public AudioClip kingCaptureSound;

    GameState currentState;
    Board<UI_Square> uiSquares;

    void Start()
    {
        uiSquares = new Board<UI_Square>(CreateSquare);
        currentStageData = LevelManager.Instance.GetStageData(LevelManager.Instance.CurrentAbsoluteLevel);

        if (currentStageData != null) currentState = PuzzleStageBuilder.CreateFromSO(currentStageData);
        else
        {
            Debug.LogError("스테이지 데이터를 불러올 수 없습니다!");
            return;
        }

        RenderState(currentState);

        UI_Square CreateSquare(BoardCoord coord)
        {
            GameObject obj = Instantiate(squarePrefab, boardPanel);
            UI_Square squareUI = obj.GetComponent<UI_Square>();
            squareUI.Init(() => OnSquareClicked(coord));
            return squareUI;
        }

        void OnSquareClicked(BoardCoord clickedCoord)
        {
            GameState nextState = currentState;

            if (currentState.ActiveSquare == null) // 선택단계
            {
                nextState = ChessPuzzleLogic.SelectStartingPiece(currentState, clickedCoord);
            }
            else
            {
                PieceType targetPiece = currentState.Board[clickedCoord];

                nextState = ChessPuzzleLogic.MoveAndTouch(currentState, clickedCoord);

                if (currentState != nextState && targetPiece != PieceType.King)
                    PlaySound(captureSound);

                if (nextState.IsVictory)
                {
                    Clear();
                    nextState = new(new Board<PieceType>(_ => PieceType.None), null);
                }
                else if (nextState.IsDefeat)
                {
                    Debug.Log("기물이 남은 상태로 킹을 잡았습니다! 스테이지를 재시작합니다.");
                    // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

            if (currentState != nextState)
            {
                currentState = nextState;
                RenderState(currentState);
            }
        }
    }

    void Clear()
    {
        PlaySound(kingCaptureSound);
        buttonPanel.Clear();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void RenderState(GameState state)
    {
        var validMoves = ChessPuzzleLogic.GetValidBatonTouches(state);
        BoardModelMapper.CreateModel(state.Board, boardTheme.lightSquareColor, boardTheme.darkSquareColor, boardTheme.PieceSpriteDict)
            .ForEach((coord, model) =>
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
}