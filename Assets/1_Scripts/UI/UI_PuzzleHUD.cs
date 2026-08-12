using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PuzzleHUD : MonoBehaviour
{
    [Header("Buttons")]
    public Button restartBtn;
    public Button toLobbyBtn;
    public Button hintBtn;
    public Button nextBtn;

    [Header("Tutorial UI")]
    public TextMeshProUGUI tutorialText;

    public event Action OnHintClicked;

    void Start()
    {
        restartBtn.onClick.AddListener(RestartStage);
        toLobbyBtn.onClick.AddListener(ToLobby);
        hintBtn.onClick.AddListener(ShowHint);
        nextBtn.onClick.AddListener(ToNextStage);

        nextBtn.gameObject.SetActive(false);

        ShowTutoriaText();
    }

    void ShowTutoriaText() => tutorialText.text = LevelManager.Instance.GetCurrentStageData().StageText;

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
        nextStage++;

        nextBtn.interactable = LevelManager.Instance.CurrentStagePlayable(nextStage);
    }

    public void InActiveHintButton() => hintBtn.interactable = false;
}