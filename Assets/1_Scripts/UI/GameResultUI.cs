using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject clearPanel;
    public Button lobbyButton;
    public Button nextStageButton;

    void Start()
    {
        lobbyButton.onClick.AddListener(GoToLobby);
        nextStageButton.onClick.AddListener(GoToNextStage);
    }

    public void OnStageCleared()
    {
        LevelManager.Instance.ClearCurrentStage();
        clearPanel.SetActive(true);
        nextStageButton.interactable = LevelManager.Instance.PlayableNextStage();
    }

    void GoToLobby() => SceneManager.LoadScene("Lobby");

    void GoToNextStage()
    {
        // 단순히 현재 레벨을 1 올리고 씬을 다시 로드하면 끝!
        LevelManager.Instance.CurrentAbsoluteLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}