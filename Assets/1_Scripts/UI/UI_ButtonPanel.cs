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
    }

    void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    void ToLobby() => SceneManager.LoadScene("Lobby");
    void ShowHint()
    {

    }

    void ToNextStage()
    {

    }
}