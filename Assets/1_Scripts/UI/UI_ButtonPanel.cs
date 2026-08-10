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

    void ToNextStage()
    {
        LevelManager.Instance.CurrentStage++;
        RestartStage();
    }

    public void Clear()
    {
        hintBtn.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(true);

        var nextStage = LevelManager.Instance.CurrentStage;
        nextStage++; // 다음 스테이지를 미리 계산

        // 바뀐 LevelManager 로직 덕분에 자물쇠와 클리어 한계가 동시에 완벽하게 계산됩니다.
        nextBtn.interactable = LevelManager.Instance.CurrentStagePlayable(nextStage);
    }

    public void InActiveHintButton() => hintBtn.interactable = false;
}