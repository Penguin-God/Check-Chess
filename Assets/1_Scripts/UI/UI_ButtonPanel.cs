using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_ButtonPanel : MonoBehaviour
{
    public Button restartBtn;
    public Button toLobbyBtn;
    public Button hintBtn;
    public Button nextBtn;

    // 힌트 버튼 클릭 이벤트를 외부(GameBoardUI)로 전달하기 위한 Action
    public event Action OnHintClicked;

    void Start()
    {
        restartBtn.onClick.AddListener(RestartStage);
        toLobbyBtn.onClick.AddListener(ToLobby);
        hintBtn.onClick.AddListener(ShowHint);
        nextBtn.onClick.AddListener(ToNextStage);
        nextBtn.gameObject.SetActive(false);
    }

    void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    void ToLobby() => SceneManager.LoadScene("Lobby");

    void ShowHint() => OnHintClicked?.Invoke();
    void ToNextStage() => RestartStage();

    public void Clear()
    {
        hintBtn.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(true);
        var status = StageStatusLogic.EvaluateStageState(LevelManager.Instance.CurrentStage, LocalStorage.LoadMaxClearedStage(), LocalStorage.LoadMaxClearableStage())
        nextBtn.interactable = status == StageState.Playable ? true : false;
        nextBtn.GetComponent<Image>().color = StageStatusLogic.GetStatusColor(status, nextBtn.GetComponent<Image>().color);
    }
}