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
    public Button toLobbyBtn;

    [Header("Theme & Visuals")]
    public BoardThemeSO boardTheme; // 생성한 SO를 여기에 할당해 주세요!

    [Header("State Colors (Logic Specific)")]
    public Color activeColor = new Color(0.73f, 0.79f, 0.27f);
    public Color validMoveColor;

    [Header("Stage Data")]
    public StageDataSO currentStageData;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip captureSound;       // 일반 기물을 잡았을 때 낼 소리
    public AudioClip kingCaptureSound;   // 킹을 잡았을 때 낼 소리

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

        restartBtn.onClick.AddListener(RestartStage);
        toLobbyBtn.onClick.AddListener(ToLobby);
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

            if (currentState.ActiveSquare == null)
            {
                nextState = ChessPuzzleLogic.SelectStartingPiece(currentState, clickedCoord);
            }
            else
            {
                // 이동하기 전, 목표 위치에 어떤 기물이 있는지 미리 확인합니다.
                PieceType targetPiece = currentState.Board[clickedCoord];

                nextState = ChessPuzzleLogic.MoveAndTouch(currentState, clickedCoord);

                // 상태가 변경되었다면 = 성공적으로 이동 및 기물을 잡았다면 사운드 재생
                if (currentState != nextState)
                {
                    if (targetPiece == PieceType.King)
                    {
                        PlaySound(kingCaptureSound);
                    }
                    else if (targetPiece != PieceType.None)
                    {
                        PlaySound(captureSound);
                    }
                }

                if (nextState.IsVictory)
                {
                    gameResultUI.gameObject.SetActive(true);
                    gameResultUI.OnStageCleared();
                }
                else if (nextState.IsDefeat)
                {
                    Debug.Log("기물이 남은 상태로 킹을 잡았습니다! 스테이지를 재시작합니다.");
                    // RestartStage();
                }
            }

            if (currentState != nextState)
            {
                currentState = nextState;
                RenderState(currentState);
            }
        }
    }

    // AudioSource가 없거나 오디오 클립이 비어있을 때 발생하는 에러를 방지하는 헬퍼 메서드
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // PlayOneShot을 사용하면 사운드가 겹치더라도 끊기지 않고 덧입혀져 재생됩니다.
            audioSource.PlayOneShot(clip);
        }
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

    public void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void ToLobby() => SceneManager.LoadScene("Lobby");
}