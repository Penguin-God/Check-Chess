using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_ButtonPanel : MonoBehaviour
{
    public Button restartBtn;
    public Button toLobbyBtn;
    public Button hintBtn;
    public Button nextBtn;

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
    void ShowHint()
    {

    }

    void ToNextStage() // 레벨을 1 올리고 씬을 다시 로드하면 됨
    {
        LevelManager.Instance.CurrentAbsoluteLevel++;
        RestartStage();
    }

    public void Clear()
    {
        hintBtn.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(true);
    }
}