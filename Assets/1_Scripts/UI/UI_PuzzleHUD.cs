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

    [Tooltip("a1, a2, a3... 순서대로 튜토리얼 텍스트를 입력하세요. 비워두면 표시되지 않습니다.")]
    [TextArea(2, 4)] // 인스펙터에서 엔터(줄바꿈)를 치기 편하도록 텍스트 입력창을 넓혀줍니다.
    public string[] tutorialMessages;

    public event Action OnHintClicked;

    void Start()
    {
        restartBtn.onClick.AddListener(RestartStage);
        toLobbyBtn.onClick.AddListener(ToLobby);
        hintBtn.onClick.AddListener(ShowHint);
        nextBtn.onClick.AddListener(ToNextStage);

        nextBtn.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(false);
        ShowTutorialIfNeeded();
    }

    void ShowTutorialIfNeeded()
    {
        StageCoord current = LevelManager.Instance.CurrentStage;

        // a챕터(ChapterIndex == 0)이고, 인스펙터에 등록된 배열의 길이보다 StageIndex가 작을 때만 실행합니다.
        if (current.ChapterIndex == 0 && current.StageIndex < tutorialMessages.Length)
        {
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = tutorialMessages[current.StageIndex];
        }
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
        nextStage++;

        nextBtn.interactable = LevelManager.Instance.CurrentStagePlayable(nextStage);
    }

    public void InActiveHintButton() => hintBtn.interactable = false;
}