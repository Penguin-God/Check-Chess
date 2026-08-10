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

    public void BindClickAction(UnityAction onClickAction, bool interactable = true)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClickAction);
        button.interactable = interactable;
    }

    public void UpdateVisuals(SquareModel squareModel)
    {
        background.color = squareModel.BgColor;
        statusIcon.sprite = squareModel.StatusIcon;
        statusIcon.color = squareModel.StatusIcon == null ? Color.clear : Color.white;
    }
}
