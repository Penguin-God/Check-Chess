using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public record SquareModel(Color BgColor, Sprite StatusIcon);

public class UI_Square : MonoBehaviour
{
    [Header("UI Components")]
    public Button button;
    public Image background;
    public Image statusIcon;

    // 초기화 및 클릭 이벤트 연결을 도와주는 헬퍼 함수
    public void Init(UnityAction onClickAction)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClickAction);
    }

    public void UpdateVisuals(Color bgColor, Sprite sprite, Color spriteColor)
    {
        background.color = bgColor;
        statusIcon.sprite = sprite;
        statusIcon.color = spriteColor;
    }

    public void UpdateVisuals(SquareModel squareModel)
    {
        background.color = squareModel.BgColor;
        statusIcon.sprite = squareModel.StatusIcon;
        statusIcon.color = squareModel.StatusIcon == null ? Color.clear : Color.white;
    }
}
